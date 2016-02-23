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
	open numeric(6,3),
	high numeric(6,3),
	low numeric(6,3),
	close numeric(6,3)
);

create index candlestick_instrument_granularity_date_time_idx on candlestick(instrument,granularity,date_time);
