using Npgsql;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Data;
using System.Text;
using EventSourcing.MVP.Infrastructure.Messaging;
using EventSourcing.MVP.Infrastructure.Store;
using EventSourcing.MVP.Infrastructure.Exceptions;

namespace EventSourcing.MVP.Postgresql;
public class PostgresqlEventStore : IEventStore
{
    private readonly string _connectionString;

    public PostgresqlEventStore(string connectionString) => _connectionString = connectionString;

    public async Task<IEnumerable<Event>> LoadEventsAsync(string aggregateType, string aggregateId, CancellationToken cancellationToken)
    {
        await using var connection = await OpenConnectionAsync(cancellationToken);
        await using var command = connection.CreateCommand();
            
        command.CommandText = "SELECT aggregate_id AS aggregateid, aggregate_type AS aggregatetype, version, payload, event_type AS eventtype FROM public.event_log WHERE aggregate_type = @aggregateType AND aggregate_id = @aggregateId ORDER BY version ASC;";
        command.Parameters.AddWithValue(nameof(aggregateType), aggregateType);
        command.Parameters.AddWithValue(nameof(aggregateId), aggregateId);

        var reader = await command.ExecuteReaderAsync(cancellationToken);
        var result = new List<Event>();

        while (await reader.ReadAsync(cancellationToken))
        {
            result.Add(new ()
            {
                AggregateType = reader.GetString(nameof(Event.AggregateType)),
                AggregateId = reader.GetString(nameof(Event.AggregateId)),
                EventType = reader.GetString(nameof(Event.EventType)),
                Payload = reader.GetString(nameof(Event.Payload)),
                Version = reader.GetInt16(nameof(Event.Version))
            });
        }

        return result;
    }

    public async Task StoreAsync(IEnumerable<Event> events, CancellationToken cancellationToken)
    {
        await using var connection = await OpenConnectionAsync(cancellationToken);
        await using var transaction = await connection.BeginTransactionAsync(cancellationToken);
        await using var command = connection.CreateCommand();

        var insertClauses = events.Select((evt, i) =>
        {
            command.Parameters.AddWithValue($"{nameof(Event.Payload)}_{i}", evt.Payload);
            command.Parameters.AddWithValue($"{nameof(Event.AggregateType)}_{i}", evt.AggregateType);
            command.Parameters.AddWithValue($"{nameof(Event.EventType)}_{i}", evt.EventType);
            command.Parameters.AddWithValue($"{nameof(Event.AggregateId)}_{i}", evt.AggregateId);
            command.Parameters.AddWithValue($"{nameof(Event.Version)}_{i}", evt.Version);
                
            return string.Format("(@Payload_{0}, @AggregateType_{0}, @AggregateId_{0}, @Version_{0}, @EventType_{0})", i);
        });

        try
        {
            command.Transaction = transaction;
            command.CommandText = string.Format("INSERT INTO public.event_log(payload, aggregate_type, aggregate_id, version, event_type) VALUES {0}", string.Join(',', insertClauses));

            await command.ExecuteNonQueryAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
        }
        catch (PostgresException ex) when (ex.SqlState == "23505")
        {
            throw ConcurrencyAggregateException.Instance;
        }
    }

    public async Task<IEnumerable<Event>> ListEventsAsync(int startFromExclusive, int batchSize, CancellationToken cancellationToken)
    {
        var result = new List<Event>(batchSize);
                
        await using var connection = await OpenConnectionAsync(cancellationToken);
        await using var command = connection.CreateCommand();
                
        command.CommandText = "SELECT id, event_type AS eventtype, payload, inserted_at AS insertedat FROM public.event_log WHERE id > @startFromExclusive ORDER BY id ASC LIMIT @batchSize;";
        command.Parameters.AddWithValue(nameof(startFromExclusive), startFromExclusive);
        command.Parameters.AddWithValue(nameof(batchSize), batchSize);

        await using var reader = await command.ExecuteReaderAsync(cancellationToken);

        while(reader.HasRows && await reader.ReadAsync(cancellationToken))
        {
            result.Add(new ()
            {
                Id = reader.GetInt32(nameof(Event.Id)),
                EventType = reader.GetString(nameof(Event.EventType)),
                Payload = reader.GetString(nameof(Event.Payload)),
                InsertedAt = reader.GetDateTime(nameof(Event.InsertedAt))
            });
        }

        return result;
    }

    private async Task<NpgsqlConnection> OpenConnectionAsync(CancellationToken cancellationToken)
    {
        var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);

        return connection;
    }
}
