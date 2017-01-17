angular.module('starter.controllers')
    .factory('windsService', function () {
        return {
            formatBearing: function(bearing) {
                if (bearing < 0 && bearing > -180) {
                    bearing = 360.0 + bearing;
                }
                if (bearing > 360 || bearing < -180) {
                    return "Unknown";
                }

                var directions = ["N", "NNE", "NE", "ENE", "E", "ESE", "SE", "SSE", "S", "SSW", "SW", "WSW", "W", "WNW", "NW", "NNW", "N"];
                var cardinal = directions[Math.floor(((bearing + 11.25) % 360) / 22.5)];
                return bearing + "\u00b0 " + cardinal;
            }
        }
    });