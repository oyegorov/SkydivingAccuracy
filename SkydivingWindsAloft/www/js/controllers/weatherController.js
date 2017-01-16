angular.module('starter.controllers')
    .controller('WeatherController', function($scope,
        $state,
        $ionicLoading,
        $ionicHistory,
        weatherService,
        settingsService,
        unitsService) {
        var loadWeather = function(latitude, longitude) {
            $ionicLoading.show({
                template: '<p>Loading...</p><ion-spinner></ion-spinner>'
            });

            weatherService.getWeather(latitude, longitude).then(function(weather) {
                function formatBearing(bearing) {
                    if (bearing < 0 && bearing > -180) {
                        bearing = 360.0 + bearing;
                    }
                    if (bearing > 360 || bearing < -180) {
                        return "Unknown";
                    }

                    var directions = [
                        "N", "NNE", "NE", "ENE", "E", "ESE", "SE", "SSE",
                        "S", "SSW", "SW", "WSW", "W", "WNW", "NW", "NNW",
                        "N"
                    ];
                    var cardinal = directions[Math.floor(((bearing + 11.25) % 360) / 22.5)];
                    return bearing + "\u00b0 " + cardinal;
                };

                var windsAloftRecords = [];

                if (weather.windsAloft != null && weather.windsAloft.windsAloftRecords != null) {
                    windsAloftRecords = weather.windsAloft.windsAloftRecords;
                    for (var i = 0; i < windsAloftRecords.length; i++) {
                        windsAloftRecords[i].bearing = formatBearing(windsAloftRecords[i].windHeading);
                    }
                }
                if (weather.groundWeather != null)
                    weather.groundWeather.bearing = formatBearing(weather.groundWeather.windHeading);

                $scope.windsAloftRecords = windsAloftRecords;
                $scope.groundWeather = weather.groundWeather;
                $scope.weatherLoaded = true;

                $ionicLoading.hide();
            });
        }

        $scope.reloadWeather = function() {
            var locationInfo = settingsService.loadLocationInfo();

            if (locationInfo != null) {
                if (locationInfo.dropzoneName != null)
                    $scope.locationName = locationInfo.dropzoneName;
                else
                    $scope.locationName = '(' + locationInfo.latitude + '; ' + locationInfo.longitude + ')';

                loadWeather(locationInfo.latitude, locationInfo.longitude);

            } else {
                $scope.weatherLoaded = false;
                $scope.dropzoneName = null;
            }
        }

        $scope.gotoLocationSettings = function() {
            $state.transitionTo('app.location');
        }

        $scope.$on('$ionicView.beforeEnter', function() {
            $scope.reloadWeather();
            $scope.unitsSettings = settingsService.loadUnitsSettings();
            $scope.unitsService = unitsService;
        });
    });