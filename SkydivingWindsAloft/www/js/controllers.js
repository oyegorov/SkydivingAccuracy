angular.module('starter.controllers', [])

.directive('rotate', function () {
    return {
        link: function (scope, element, attrs) {
            // watch the degrees attribute, and update the UI when it changes
            scope.$watch(attrs.degrees,
                function (rotateDegrees) {
                    console.log(rotateDegrees);
                    //transform the css to rotate based on the new rotateDegrees
                    element.css({
                        'transform': 'rotate(' + rotateDegrees + 'deg)',
                        '-moz-transform': 'rotate(' + rotateDegrees + 'deg)',
                        '-webkit-transform': 'rotate(' + rotateDegrees + 'deg)',
                        '-o-transform': 'rotate(' + rotateDegrees + 'deg)',
                        '-ms-transform': 'rotate(' + rotateDegrees + 'deg)'
                    });
                });
        }
    }
})

.factory('weatherService', function ($http) {
    var weather = [];

    return {
        getWeather: function (longitude, latitude) {
            return $http.get("http://vps96817.vps.ovh.ca/api/weather/currentWeather?longitude=" + longitude + "&latitude=" + latitude).then(function (response) {
                weather = response;
                return weather;
            }, function (error) {
                //there was an error fetching from the server
            });
        }
    }
})

.controller('WeatherController', function ($scope, weatherService) {
    $scope.loadWeather = function () {
        weatherService.getWeather(-79.6115, 44.3009).then(function (response) {
            function formatBearing(bearing) {
                if (bearing < 0 && bearing > -180) {
                    bearing = 360.0 + bearing;
                }
                if (bearing > 360 || bearing < -180) {
                    return "Unknown";
                }

                var directions = ["N", "NNE", "NE", "ENE", "E", "ESE", "SE", "SSE",
                    "S", "SSW", "SW", "WSW", "W", "WNW", "NW", "NNW",
                    "N"];
                var cardinal = directions[Math.floor(((bearing + 11.25) % 360) / 22.5)];
                return bearing + "\u00b0 " + cardinal;
            };

            var windsAloftRecords = response.data.windsAloft.windsAloftRecords;
            for (var i = 0; i < windsAloftRecords.length; i++) {
                windsAloftRecords[i].bearing = formatBearing(windsAloftRecords[i].windHeading);
            }
            response.data.groundWeather.bearing = formatBearing(response.data.groundWeather.windHeading);

            $scope.windsAloftRecords = windsAloftRecords;
            $scope.groundWeather = response.data.groundWeather;
        });
    }

    $scope.loadWeather();
});
