create table order_book (
	id bigserial primary key,
	date_time timestamp with time zone,
	rate numeric(6,3)
);

create index order_book_date_time_idx on order_book(date_time desc);

create table order_book_price_point (
	id bigserial primary key,
	order_book_id bigint,
	price numeric(6,3),
	os numeric(6,4),
	ps numeric(6,4),
	ol numeric(6,4),
	pl numeric(6,4)
);

create index order_book_price_point_order_book_idx on order_book_price_point(order_book_id);

create table candlestick (
	id bigserial primary key,
	instrument text,
	granularity text,
	date_time timestamp with time zone,
	open numeric(12,5),
	high numeric(12,5),
	low numeric(12,5),
	close numeric(12,5),
	volume integer,
	unique(instrument,granularity,date_time)
);

alter table candlestick alter column open type numeric(12,5);
alter table candlestick alter column high type numeric(12,5);
alter table candlestick alter column low type numeric(12,5);
alter table candlestick alter column close type numeric(12,5);

create table time_of_day_pattern (
	id bigserial primary key,
	check_start_time time,
	check_end_time time,
	is_check_up boolean,
	trade_start_time time,
	trade_end_time time,
	trade_type text,
	total_verification integer,
	match_verification integer
);
