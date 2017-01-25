angular.module('starter.controllers')
    .controller('WeatherController', function($scope,
        $state,
        $ionicLoading,
        $ionicHistory,
        weatherService,
        settingsService,
        unitsService) {

        $scope.weatherService = weatherService;

        $scope.loadWeather = function(force) {
            $scope.errorLoadingWeather = false;
            var locationInfo = settingsService.loadLocationInfo();

            if (locationInfo != null) {
                if (locationInfo.dropzoneName != null)
                    $scope.locationName = locationInfo.dropzoneName;
                else
                    $scope.locationName = '(' + locationInfo.latitude + '; ' + locationInfo.longitude + ')';
            }

            weatherService.loadWeather(locationInfo, force,
                function onStartLoading() {
                    $ionicLoading.show({
                        template: '<p>Loading...</p><ion-spinner></ion-spinner>'
                    });
                },
                function onEndLoading() {
                    $ionicLoading.hide();
                }
            );
        }

        $scope.gotoLocationSettings = function() {
            $state.transitionTo('app.location');
        }

        $scope.$on('$ionicView.beforeEnter', function() {
            $scope.loadWeather(false);
            $scope.unitsSettings = settingsService.loadUnitsSettings();
            $scope.unitsService = unitsService;
        });
    });