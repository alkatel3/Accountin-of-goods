CREATE TABLE IF NOT EXISTS "__EFMigrationsHistory" (
    "MigrationId" TEXT NOT NULL CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY,
    "ProductVersion" TEXT NOT NULL
);

BEGIN TRANSACTION;

CREATE TABLE "Categories" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_Categories" PRIMARY KEY AUTOINCREMENT,
    "Name" TEXT NOT NULL
);

CREATE TABLE "Goods" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_Goods" PRIMARY KEY AUTOINCREMENT,
    "Name" TEXT NOT NULL,
    "Count" INTEGER NOT NULL,
    "Priсe" TEXT NOT NULL,
    "CategoryId" INTEGER NULL,
    CONSTRAINT "FK_Goods_Categories_CategoryId" FOREIGN KEY ("CategoryId") REFERENCES "Categories" ("Id") ON DELETE CASCADE
);

CREATE INDEX "IX_Goods_CategoryId" ON "Goods" ("CategoryId");

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20221106172826_FirstMigration', '7.0.0-rc.2.22472.11');

COMMIT;

BEGIN TRANSACTION;

CREATE TABLE "ef_temp_Goods" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_Goods" PRIMARY KEY AUTOINCREMENT,
    "CategoryId" INTEGER NULL,
    "Count" INTEGER NOT NULL,
    "Name" TEXT NOT NULL,
    "Priсe" TEXT NOT NULL,
    CONSTRAINT "FK_Goods_Categories_CategoryId" FOREIGN KEY ("CategoryId") REFERENCES "Categories" ("Id")
);

INSERT INTO "ef_temp_Goods" ("Id", "CategoryId", "Count", "Name", "Priсe")
SELECT "Id", "CategoryId", "Count", "Name", "Priсe"
FROM "Goods";

COMMIT;

PRAGMA foreign_keys = 0;

BEGIN TRANSACTION;

DROP TABLE "Goods";

ALTER TABLE "ef_temp_Goods" RENAME TO "Goods";

COMMIT;

PRAGMA foreign_keys = 1;

BEGIN TRANSACTION;

CREATE INDEX "IX_Goods_CategoryId" ON "Goods" ("CategoryId");

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20221107021044_SecontMigration', '7.0.0-rc.2.22472.11');

COMMIT;

