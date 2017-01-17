angular.module('starter.controllers')
    .controller('WeatherController', function($scope,
        $state,
        $ionicLoading,
        $ionicHistory,
        weatherService,
        settingsService,
        unitsService,
        windsService) {
        var loadWeather = function(latitude, longitude) {
            $ionicLoading.show({
                template: '<p>Loading...</p><ion-spinner></ion-spinner>'
            });

            weatherService.getWeather(latitude, longitude).then(function(weather) {
                if (weather != null) {
                    var windsAloftRecords = [];

                    if (weather.windsAloft != null && weather.windsAloft.windsAloftRecords != null) {
                        windsAloftRecords = weather.windsAloft.windsAloftRecords;
                        for (var i = 0; i < windsAloftRecords.length; i++) {
                            windsAloftRecords[i].bearing = windsService.formatBearing(windsAloftRecords[i].windHeading);
                        }
                    }
                    if (weather.groundWeather != null)
                        weather.groundWeather.bearing = windsService.formatBearing(weather.groundWeather.windHeading);

                    $scope.windsAloftRecords = windsAloftRecords;
                    $scope.groundWeather = weather.groundWeather;
                    $scope.weatherLoaded = true;
                } else {
                    $scope.errorLoadingWeather = true;
                }

                $ionicLoading.hide();
            });
        }

        $scope.reloadWeather = function() {
            $scope.errorLoadingWeather = false;
            var locationInfo = settingsService.loadLocationInfo();

            if (locationInfo != null) {
                if (locationInfo.dropzoneName != null)
                    $scope.locationName = locationInfo.dropzoneName;
                else
                    $scope.locationName = '(' + locationInfo.latitude + '; ' + locationInfo.longitude + ')';

                $scope.locationPresent = true;

                loadWeather(locationInfo.latitude, locationInfo.longitude);

            } else {
                $scope.locationPresent = false;
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