create table if not exists apikeys(
    apikey varchar(100) primary key,
    expires timestamp with time zone not null default current_timestamp
)

create table if not exists prices(
    id serial primary key,
    quotename varchar(30) not null,
    taken timestamp with time zone not null default current_timestamp,
    price decimal(18,4) not null
)

insert into apikeys(apikey, expires)
values ('test-api-key', '2026-12-31');