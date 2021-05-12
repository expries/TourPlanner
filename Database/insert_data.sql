INSERT INTO tour (name, locationfrom, locationto, description, distance, tourType)
    VALUES ('Standard', 'Wien', 'Graz', 'Real fun', 165.3, 'fastest');

INSERT INTO tour (name, locationfrom, locationto, description, distance, tourType)
    VALUES ('Berg-Tour', 'Eisenstadt', 'Wien', 'Schwierig, aber lustig', 41.82, 'bicycle');

INSERT INTO tour (name, locationfrom, locationto, description, distance, tourType)
    VALUES ('West-Tour', 'Salzburg', 'Villach', 'Nun ja..', 145.05, 'shortest');

INSERT INTO tour_log (starttime, endtime, rating, fk_tourid)
    VALUES ('2016-06-22 19:10:25-07', '2016-06-22 19:10:25-07', 5, 1);

INSERT INTO tour_log (starttime, endtime, rating, fk_tourid)
    VALUES ('2016-06-22 19:10:25-07', '2016-06-22 19:10:25-07', 5, 2);

INSERT INTO tour_log (starttime, endtime, rating, fk_tourid)
    VALUES ('2016-06-22 19:10:25-07', '2016-06-22 19:10:25-07', 5, 2);

INSERT INTO tour_log (starttime, endtime, rating, fk_tourid)
    VALUES ('2016-06-22 19:10:25-07', '2016-06-22 19:10:25-07', 1, 3);