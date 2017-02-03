angular.module('starter.controllers')
    .controller('MapController', function ($scope, $state, $ionicLoading, settingsService, weatherService, unitsService, spottingService) {
        $scope.weatherService = weatherService;

        $scope.loadWeather = function (force) {
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
                    $scope.weather = weatherService.weather;
                },
                function onLoadNotNeeded() {
                    $scope.weather = weatherService.weather;
                }
            );
        }
        
        $scope.loadLocationInfo = function() {
            $scope.locationInfo = settingsService.loadLocationInfo();
            $scope.spot = new google.maps.LatLng($scope.locationInfo.latitude, $scope.locationInfo.longitude);

            if ($scope.locationInfo == null) {
                $scope.locationPresent = false;
                $scope.dropzoneName = null;
                return;
            }

            $scope.locationPresent = true;
            if ($scope.locationInfo.dropzoneName != null)
                $scope.locationName = $scope.locationInfo.dropzoneName;
            else
                $scope.locationName = '(' + $scope.locationInfo.latitude + '; ' + $scope.locationInfo.longitude + ')';
        }

        $scope.$on('$ionicView.beforeEnter', function () {
            $scope.loadLocationInfo();
            $scope.unitsSettings = settingsService.loadUnitsSettings();
            $scope.loadWeather(false);
        });

        $scope.gotoLocationSettings = function () {
            $state.transitionTo('app.location');
        }
    });