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
    IMAGE VARCHAR(255) DEFAULT NULL,
    CONSTRAINT Unique_Utensil UNIQUE(NAME)
);

CREATE TABLE Category(
    ID INT PRIMARY KEY AUTO_INCREMENT NOT NULL,
    NAME VARCHAR(50) NOT NULL
);

CREATE TABLE Achievement( 
    ID INT PRIMARY KEY AUTO_INCREMENT NOT NULL,
    NAME VARCHAR(255) NOT NULL,
    DESCRIPTION VARCHAR(255) NOT NULL,
    IMAGE VARCHAR(255) DEFAULT NULL,
    CONSTRAINT Unique_Achievement UNIQUE(NAME)
);

CREATE TABLE Ingredient(
    ID INT PRIMARY KEY AUTO_INCREMENT NOT NULL,
    NAME VARCHAR(50) NOT NULL,
    IMAGE VARCHAR(255) DEFAULT NULL,
    CATEGORY INT NOT NULL,
    CONSTRAINT fk_Category
        FOREIGN KEY (CATEGORY) REFERENCES Category(ID),
    CONSTRAINT Unique_Ingredient UNIQUE(NAME)
);

CREATE TABLE User (
    ID INT PRIMARY KEY AUTO_INCREMENT NOT NULL,
    NAME VARCHAR(255) NOT NULL,
    PASSWORD VARCHAR(255) NOT NULL,
    LEVEL TINYINT DEFAULT 1 NOT NULL,
    XP INT DEFAULT 1 NOT NULL,
    EMAIL VARCHAR(255) NOT NULL,
    IMAGE VARCHAR(255) DEFAULT 'Images/Placeholder.png',
    UNIQUE (NAME),         
    UNIQUE (EMAIL),        
    CONSTRAINT Unique_User UNIQUE (NAME, EMAIL) 
);

CREATE TABLE Recipe(
    ID INT PRIMARY KEY AUTO_INCREMENT NOT NULL,
    NAME VARCHAR(255) NOT NULL,
    TIME INT NOT NULL,
    DIFFICULTY INT NOT NULL,
    INSTRUCTIONS VARCHAR(5000),
    RATING TINYINT DEFAULT 0,
    RATINGCOUNT INT DEFAULT 0,
    CREATEDBY INT DEFAULT 0 NOT NULL,
    IMAGE VARCHAR(255) DEFAULT NULL,
    CONSTRAINT fk_Difficulty
        FOREIGN KEY (DIFFICULTY) REFERENCES Difficulty(ID),
    CONSTRAINT fk_User
        FOREIGN KEY (CREATEDBY) REFERENCES User(ID)
);

CREATE TABLE Recipe_Ingredient(
    ID INT PRIMARY KEY AUTO_INCREMENT NOT NULL,
    RECIPEID INT NOT NULL,
    INGREDIENTID INT NOT NULL,
    QUANTITY FLOAT NOT NULL,
    CONSTRAINT fk_I_RecipeID
        FOREIGN KEY (RECIPEID) REFERENCES Recipe(ID) ON DELETE CASCADE,
    CONSTRAINT fk_IngredientID
        FOREIGN KEY (INGREDIENTID) REFERENCES Ingredient(ID)
);

CREATE TABLE Recipe_Utensil(
    ID INT PRIMARY KEY AUTO_INCREMENT NOT NULL,
    RECIPEID INT NOT NULL,
    UTENSILID INT NOT NULL,
    QUANTITY INT NOT NULL,
    CONSTRAINT fk_U_RecipeID
        FOREIGN KEY (RECIPEID) REFERENCES Recipe(ID) ON DELETE CASCADE,
    CONSTRAINT fk_UtensilID
        FOREIGN KEY (UTENSILID) REFERENCES Utensil(ID)
);

CREATE TABLE User_Achievement(  
    ID INT PRIMARY KEY AUTO_INCREMENT NOT NULL,
    USERID INT NOT NULL,
    ACHIEVEMENTID INT NOT NULL,  
    TIME TIMESTAMP DEFAULT CURRENT_TIMESTAMP NOT NULL,  
    CONSTRAINT fk_UserID
        FOREIGN KEY (USERID) REFERENCES User(ID),
    CONSTRAINT fk_AchievementID  
        FOREIGN KEY (ACHIEVEMENTID) REFERENCES Achievement(ID)
);

INSERT INTO Difficulty(NAME) VALUES ('Leicht'), ('Mittel'), ('Schwer');

INSERT INTO User(NAME, EMAIL, PASSWORD, LEVEL, XP) VALUES
('Niklas', 'Niklas@email.de', 'Admin',5,20),
('Florian', 'Florian@email.de', 'Admin',7,4),
('Cornelius', 'Cornelius@email.de', 'Admin',1,15),
('Jonas', 'Jonas@email.de', 'Admin',10,70);

INSERT INTO Utensil (NAME, IMAGE) VALUES 
('Löffel', '\\Bilder\\x.png'),
('Messer', '\\Bilder\\x.png'),
('Schneidebrett', '\\Bilder\\x.png'),
('Pfanne', '\\Bilder\\x.png'),
('Rührbesen', '\\Bilder\\x.png'),
('Schüssel', '\\Bilder\\x.png');

INSERT INTO Category (NAME) VALUES ('Unkategorisiert'), ('Obst/Gemuese'), ('Fleisch'), ('Milchprodukte');

INSERT INTO Ingredient (NAME, CATEGORY, IMAGE) VALUES 
('Mehl', 1, '\\Bilder\\x.png'),
('Zucker', 1, '\\Bilder\\x.png'),
('Eier', 1, '\\Bilder\\x.png'),
('Milch', 4, '\\Bilder\\x.png'),
('Butter', 4, '\\Bilder\\x.png'),
('Tomate', 2, '\\Bilder\\x.png');

INSERT INTO Achievement (NAME,DESCRIPTION,IMAGE) VALUES
('Starter','Hat das erste Rezept gekocht','bild.png'),
('Kleiner Koch','Hat 5 Rezepte gekocht','bild.png'),
('Ueberflieger','Hat 10 Rezepte gekocht','bild.png');

INSERT INTO User_Achievement(USERID,ACHIEVEMENTID) VALUES
(4,1),
(4,2);
-- Rezept für Pfannkuchen
INSERT INTO Recipe(NAME, TIME, DIFFICULTY, INSTRUCTIONS, CREATEDBY, IMAGE)
VALUES ('Pfannkuchen',20,2,'Alle Zutaten vermengen und in der Pfanne ausbacken.', 1, '\\Bilder\\x.png');
-- Zutaten speichern
INSERT INTO Recipe_Ingredient(RECIPEID, INGREDIENTID, QUANTITY)
VALUES 
(1, 1, 200),   -- 200g Mehl
(1, 2, 50),    -- 50g Zucker
(1, 3, 2),     -- 2 Eier
(1, 4, 250),   -- 250ml Milch
(1, 5, 50);    -- 50g Butter
-- Utensilien speichern
INSERT INTO Recipe_Utensil (RECIPEID, UTENSILID, QUANTITY)
VALUES 
(1, 1, 1),  -- 1x Löffel
(1, 2, 1),  -- 1x Messer
(1, 3, 1),  -- 1x Schneidebrett
(1, 4, 1);  -- 1x Pfanne

-- select * from Recipe where ID = 1;
-- select * from Recipe_Utensil ru join Utensil u on ru.UTENSILID = u.ID where ru.RECIPEID = 1;
-- select * from Recipe_Ingredient ri join Ingredient i on ri.INGREDIENTID = i.ID where ri.RECIPEID = 1;