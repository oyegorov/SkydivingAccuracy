angular.module('starter.controllers')
    .factory('weatherService', function($http) {
        var weather = [];

        return {
            getWeather: function(latitude, longitude) {
                var url = "http://vps96817.vps.ovh.ca/api/weather?longitude=" + longitude + "&latitude=" + latitude;

                return $http.get(url, { timeout: 12000 }).then(function(response) {
                    weather = response.data;
                    return weather;
                }, function(error) {
                    return null;
                });
            }
        }
    });