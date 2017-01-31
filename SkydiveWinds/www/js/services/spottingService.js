angular.module('starter.controllers')
    .factory('spottingService', function () {
        return {
            offsetCoordinatesBy: function (latLng, d, angle) {
                var R = 6371e3;

                var angleRad = (360 - angle) * Math.PI / 180;
                var lat1 = latLng.lat() * Math.PI / 180;
                var lng1 = latLng.lng() * Math.PI / 180;
                var angDistance = d / R;

                var lat2 = Math.asin(Math.sin(lat1) * Math.cos(angDistance) + Math.cos(lat1) * Math.sin(angDistance) * Math.cos(angleRad));
                var dlon = Math.atan2(Math.sin(angleRad) * Math.sin(angDistance) * Math.cos(lat1),
                    Math.cos(angDistance) - Math.sin(lat1) * Math.sin(lat2));
                var lng2 = lng1 - dlon + Math.PI % (2 * Math.PI) - Math.PI;

                return new google.maps.LatLng(lat2 * 180 / Math.PI, (lng2 * 180 / Math.PI + 540) % 360 - 180);
            },

            getSpotInformation: function (weather, latLng, exitAltitude) {
                var w = weather.windsAloftRecords;
                if (w == null || w.length == 0)
                    return null;

                var driftAngle = (weather.groundWeather.windHeading + w[0].windHeading) / 2;

                var K = 25;
                var A = 3;
                var V = (weather.groundWeather.windSpeed + w[0].windSpeed) / 2;
                var D = K * A * V;

                var p = this.offsetCoordinatesBy(latLng, D, driftAngle);

                K = 3;
                for (var i = 1; i < w.length && w[i].altitude <= exitAltitude; i++) {
                    driftAngle = (w[i].windHeading + w[i - 1].windHeading) / 2;

                    A = (w[i].altitude - w[i - 1].altitude) / 1000;
                    V = (w[i].windSpeed + w[i - 1].windSpeed) / 2;

                    D = K * A * V;
                    p = this.offsetCoordinatesBy(p, D, driftAngle);
                }

                D = 150;
                driftAngle = w[w.length - 1].windHeading;
                p = this.offsetCoordinatesBy(p, D, driftAngle);

                return {
                    spotCenter: p
                }
            }
        }
    });