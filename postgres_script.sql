CREATE TABLE event_log
(
	id BIGSERIAL PRIMARY KEY, 
	payload CHARACTER VARYING(MAX) NOT NULL,
	aggregate_type CHARACTER VARYING(100) NOT NULL, 
	aggregate_id CHARACTER VARYING(100) NOT NULL,
	event_type CHARACTER VARYING(100) NOT NULL,
	inserted_at TIMESTAMP WITHOUT TIME ZONE NOT NULL DEFAULT (NOW() AT TIME ZONE 'utc'),
	version SMALLINT NOT NULL
);
CREATE UNIQUE INDEX uq_aggregate_version ON event_log (aggregate_type, aggregate_id, version DESC); 
CREATE RULE event_log_prevent_delete AS ON DELETE TO event_log DO INSTEAD NOTHING;
CREATE RULE event_log_prevent_update AS ON UPDATE TO event_log DO INSTEAD NOTHING;
