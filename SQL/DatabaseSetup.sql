DROP DATABASE IF EXISTS KuechenQuest;

CREATE DATABASE KuechenQuest;

USE KuechenQuest;

CREATE TABLE Difficulty(
    ID INT PRIMARY KEY AUTO_INCREMENT NOT NULL,
    NAME VARCHAR(50) NOT NULL
);
CREATE TABLE Utensil(
    ID INT PRIMARY KEY AUTO_INCREMENT NOT NULL,
    NAME VARCHAR(50) NOT NULL,
    IMAGE LONGBLOB
);
CREATE TABLE Category(
    ID INT PRIMARY KEY AUTO_INCREMENT NOT NULL,
    NAME VARCHAR(50) NOT NULL
);
CREATE TABLE Ingredient(
    ID INT PRIMARY KEY AUTO_INCREMENT NOT NULL,
    NAME VARCHAR(50) NOT NULL,
    CATEGORY INT NOt NULL,
    IMAGE LONGBLOB,
    CONSTRAINT fk_Category
        FOREIGN KEY (CATEGORY) REFERENCES Category(ID)
);
CREATE TABLE Recipe(
    ID INT PRIMARY KEY AUTO_INCREMENT NOT NULL,
    NAME VARCHAR(255) NOT NULL,
    INGREDIENTS VARCHAR(255) DEFAULT '{}' NOT NULL,
    I_QUANTITIES VARCHAR(255) DEFAULT '{}' NOT NULL,
    TIME INT NOT NULL,
    DIFFICULTY INT NOT NULL,
    INSTRUCTIONS VARCHAR(500),
    RATING TINYINT,
    RATINGCOUNT INT DEFAULT 0,
    UTENSILS VARCHAR(255) DEFAULT '{}' NOT NULL,
    U_QUANTITIES VARCHAR(255) DEFAULT '{}' NOT NULL,
    IMAGE LONGBLOB,
    CONSTRAINT fk_Difficulty
        FOREIGN KEY (Difficulty) REFERENCES Difficulty(ID)
);
CREATE TABLE Achievment(
    ID INT PRIMARY KEY AUTO_INCREMENT NOT NULL,
    NAME VARCHAR(255) NOT NULL,
    IMAGE LONGBLOB
);
CREATE TABLE User(
    ID INT PRIMARY KEY AUTO_INCREMENT NOT NULL,
    NAME VARCHAR(255) NOT NULL,
    PASSWORD VARCHAR(255) NOT NULL,
    LEVEL TINYINT DEFAULT 0 NOT NULL,
    XP INT DEFAULT 0 NOT NULL,
    EMAIL VARCHAR(255)
);

INSERT INTO Difficulty(NAME) VALUES('Leicht'),('Mittel'),('Schwer');

INSERT INTO User(NAME,EMAIL,PASSWORD) VALUES('Niklas','Niklas@email.de','Admin');
INSERT INTO User(NAME,EMAIL,PASSWORD) VALUES('Florian','Florian@email.de','Admin');
INSERT INTO User(NAME,EMAIL,PASSWORD) VALUES('Cornelius','Cornelius@email.de','Admin');
INSERT INTO User(NAME,EMAIL,PASSWORD) VALUES('Jonas','Jonas@email.de','Admin');

INSERT INTO Utensil (NAME, IMAGE) VALUES 
('Löffel', NULL),
('Messer', NULL),
('Schneidebrett', NULL),
('Pfanne', NULL),
('Rührbesen', NULL);

INSERT INTO Category (NAME) VALUES ('Unkategorisiert'),('Obst/Gemuese'),('Fleisch'),('Milchprodukte');

INSERT INTO Ingredient (NAME, CATEGORY, IMAGE) VALUES 
('Mehl', 1, NULL),
('Zucker', 1, NULL),
('Eier', 1, NULL),
('Milch', 3, NULL),
('Butter', 3, NULL);