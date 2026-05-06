

CREATE TABLE app.machineGroup (
    id                     BIGINT             PRIMARY KEY IDENTITY (10000, 1) NOT NULL,
	orgNr                  INT             NOT NULL,
	code                   NVARCHAR (100)  NOT NULL,
	descr                  NVARCHAR (MAX)  NULL,
	encoded                NVARCHAR (MAX)  NULL,
	updated                DATETIME        NOT NULL DEFAULT GETDATE(),
	isActive    BIT             NOT NULL DEFAULT 1
	FOREIGN KEY (orgNr)    REFERENCES      base.org(nr)
);
CREATE TABLE app.machine (
    id                     BIGINT             PRIMARY KEY IDENTITY (10000, 1) NOT NULL,
	orgNr                  INT             NOT NULL,
	classification         INT             NULL,
	machineGroupId         BIGINT             NULL,
	stationPairs           INT             NOT NULL,
	code                   NVARCHAR (100)  NOT NULL,
	descr                  NVARCHAR (MAX)  NULL,
	encoded                NVARCHAR (MAX)  NULL,
	updated                DATETIME        NOT NULL DEFAULT GETDATE(),
	isActive    BIT             NOT NULL DEFAULT 1
	FOREIGN KEY (orgNr)    REFERENCES      base.org(nr),
	FOREIGN KEY (machineGroupId) REFERENCES app.machineGroup(id)
);


/*
create table country
(
	id bigserial primary key,
	image varchar
) INHERITS (app.baseref);
alter table country OWNER to postgres;
alter sequence country_id_seq restart with 10000;

create table currency
(
	id bigserial primary key	
) INHERITS (app.baseref);
alter table currency OWNER to postgres;
alter sequence currency_id_seq restart with 10000;
*/