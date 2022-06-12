CREATE TABLE "Address" (
  "id" uuid NOT NULL,
  "member_id" uuid NOT NULL,
  "phone_number" varchar(10) NOT NULL,
  "zip" varchar(50) NOT NULL,
  "region" varchar(50) NOT NULL,
  "city" varchar(50) NOT NULL,
  "street" varchar(50) NOT NULL,
  "apartment" varchar(5),
  PRIMARY KEY ("id")
);

CREATE TABLE "Event" (
  "id" uuid NOT NULL,
  "description" varchar(140),
  "end_registration" date,
  "end_event" date,
  "sum_price" int4,
  "send_friends" bool DEFAULT false,
  "tracking" bool DEFAULT false,
  "reshuffle" bool DEFAULT false,
  PRIMARY KEY ("id")
);

CREATE TABLE "Member" (
  "id" uuid NOT NULL,
  "name" varchar(50),
  "surname" varchar(50),
  "patronymic" varchar(50),
  "email" varchar(50),
  "role" varchar(50),
  "login" varchar(100) NOT NULL,
  PRIMARY KEY ("id")
);

CREATE TABLE "Member_Event" (
  "id" uuid NOT NULL,
  "member_id" uuid NOT NULL,
  "event_id" uuid NOT NULL,
  "member_attend" bool DEFAULT true,
  "delivery_service" varchar(100),
  "track_number" varchar(100),
  "preference" text,
  "recipient" uuid,
  "send_day" date,
  PRIMARY KEY ("id")
);

ALTER TABLE "Address" ADD CONSTRAINT "fk_Address_Member_1" FOREIGN KEY ("member_id") REFERENCES "Member" ("id");
ALTER TABLE "Member_Event" ADD CONSTRAINT "fk_Member_Event_Member_1" FOREIGN KEY ("member_id") REFERENCES "Member" ("id");
ALTER TABLE "Member_Event" ADD CONSTRAINT "fk_Member_Event_Event_1" FOREIGN KEY ("event_id") REFERENCES "Event" ("id");

INSERT INTO public."Member" (id,"name",surname,patronymic,email,"role",login)
	VALUES ('cee00a1e-54aa-410a-9717-11165e027c15'::uuid,'admin','admin','admin','admin','admin','admin');

