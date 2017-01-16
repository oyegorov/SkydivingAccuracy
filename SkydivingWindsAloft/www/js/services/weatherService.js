angular.module('starter.controllers')
    .factory('weatherService', function($http) {
        var weather = [];

        return {
            getWeather: function(latitude, longitude) {
                return $http.get("http://vps96817.vps.ovh.ca/api/weather?longitude=" +
                    longitude +
                    "&latitude=" +
                    latitude).then(function(response) {
                    weather = response.data;
                    return weather;
                }, function(error) {
                    return null;
                    //there was an error fetching from the server
                });
            }
        }
    });