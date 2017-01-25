angular.module('starter.controllers')
    .factory('weatherService', function ($http, globalConfigurationService) {
        var weather = [];

        return {
            getWeather: function(latitude, longitude) {
                var url = globalConfigurationService.backendApiBaseUrl + "/weather?longitude=" + longitude + "&latitude=" + latitude;

                return $http.get(url, { timeout: 12000 }).then(function(response) {
                    weather = response.data;
                    return weather;
                }, function(error) {
                    return null;
                });
            }
        }
    });