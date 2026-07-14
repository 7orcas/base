SELECT
	entity,
	table_schema,
    --table_name,
    comment_text
FROM (
    -- Table comments first
    SELECT
		t.table_schema,
		t.table_name as entity,
        t.table_name,
        0 AS sort_order,
        obj_description(
            (quote_ident(t.table_schema) || '.' || quote_ident(t.table_name))::regclass,
            'pg_class'
        ) AS comment_text
    FROM information_schema.tables t
    WHERE (t.table_schema = 'cntrl'
	   OR t.table_schema = 'base'
	   OR t.table_schema = 'app')
      AND t.table_type = 'BASE TABLE'

    UNION ALL

    -- Column comments
    SELECT
		c.table_schema,
        ' - ' || c.column_name as entity,
		c.table_name || '.' || c.column_name as table_name,
        c.ordinal_position AS sort_order,
        COALESCE(
            col_description(
                (quote_ident(c.table_schema) || '.' || quote_ident(c.table_name))::regclass,
                c.ordinal_position
            ),
            ''
        ) AS comment_text
    FROM information_schema.columns c
    WHERE c.table_schema = 'cntrl'
	OR    c.table_schema = 'base'
	OR    c.table_schema = 'app'
) x
WHERE LENGTH(comment_text) > 0
ORDER BY
    table_name,
    sort_order;