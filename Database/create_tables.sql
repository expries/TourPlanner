CREATE TYPE tour_type AS ENUM ('fastest', 'shortest', 'pedestrian', 'bicycle');
CREATE TYPE difficulty_type AS ENUM ('very_easy', 'easy', 'medium', 'hard', 'very_hard');
CREATE TYPE weather_type AS ENUM ('cloudy', 'rainy', 'windy', 'sunny');

CREATE TABLE tour (
    tourId SERIAL PRIMARY KEY,
    name VARCHAR NOT NULL,
    locationFrom VARCHAR NOT NULL,
    locationTo VARCHAR NOT NULL,
    tourType tour_type NOT NULL,
    distance FLOAT NOT NULL,
    description VARCHAR NOT NULL,
    imagePath VARCHAR NOT NULL DEFAULT 'C:\Users\Samuel\Documents\TourPlanner-Images\example.png'
);

CREATE TABLE tour_log (
    tourLogId SERIAL PRIMARY KEY,
    date DATE NOT NULL,
    duration FLOAT NOT NULL,
    distance FLOAT NOT NULL,
    rating INT NOT NULL,
    temperature FLOAT NOT NULL,
    averageSpeed FLOAT NOT NULL,
    dangerLevel INT NOT NULL,
    difficulty difficulty_type NOT NULL,
    weather weather_type NOT NULL,
    fk_tourID INT NOT NULL,
    CHECK (rating BETWEEN 1 AND 5),
    CHECK (dangerLevel BETWEEN 1 AND 5),
    CHECK (temperature BETWEEN -45 AND 45),
    CHECK (averageSpeed BETWEEN 0 AND 300),
    CHECK (distance > 0),
    CHECK (duration > 0),
    FOREIGN KEY (fk_tourID) REFERENCES tour(tourId) ON DELETE CASCADE
);
