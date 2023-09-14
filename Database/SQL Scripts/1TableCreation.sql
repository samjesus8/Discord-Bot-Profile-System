create schema if not exists data;

create table if not exists data."userinfo"
(
	"userno" bigint primary key,
	"username" varchar not null,
	"serverid" bigint not null,
	"avatarurl" varchar not null,
	"level" bigint not null,
	"xp" bigint not null,
	"xplimit" bigint not null
);