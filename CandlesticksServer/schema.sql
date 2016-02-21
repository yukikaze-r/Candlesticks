create table order_book (
	id bigserial primary key,
	date_time timestamp with time zone,
	rate numeric(6,3)
);

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

