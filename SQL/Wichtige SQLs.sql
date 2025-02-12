SELECT 
    r.ID,
    r.NAME,
    r.TIME,
    d.NAME as 'DNAME',
    r.INSTRUCTIONS,
    r.RATING,
    r.RATINGCOUNT,
    r.IMAGE
FROM Recipe r
JOIN Difficulty d ON r.DIFFICULTY = d.ID
WHERE r.ID = 1;

SELECT 
    ru.ID,
    ru.RECIPEID,
    ru.UTENSILID,
    u.NAME,
    ru.QUANTITY,
    u.IMAGE
FROM Recipe_Utensil ru
JOIN Utensil u ON ru.UTENSILID = u.ID
WHERE ru.RECIPEID = 1;

SELECT 
    ri.ID,
    ri.RECIPEID,
    ri.INGREDIENTID,
    i.NAME,
    ri.QUANTITY,
    c.NAME as 'CNAME',
    i.IMAGE
FROM Recipe_Ingredient ri
JOIN Ingredient i ON ri.INGREDIENTID = i.ID
JOIN Category c ON i.CATEGORY = c.ID
WHERE ri.RECIPEID = 1;
