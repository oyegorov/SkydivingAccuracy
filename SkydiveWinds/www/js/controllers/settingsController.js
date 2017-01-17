angular.module('starter.controllers')
    .controller('SettingsController', function($scope, settingsService, unitsService) {
        $scope.temperatureUnits = unitsService.getTemperatureUnits();
        $scope.windSpeedUnits = unitsService.getWindSpeedUnits();
        $scope.altitudeUnits = unitsService.getAltitudeUnits();
        $scope.distanceUnits = unitsService.getDistanceUnits();

        $scope.onUnitsChanged = function() {
            settingsService.saveUnitsSettings($scope.unitsSettings);
        }

        $scope.$on('$ionicView.beforeEnter', function() {
            $scope.unitsSettings = settingsService.loadUnitsSettings();
        });
    });