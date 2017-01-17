angular.module('starter.controllers')
    .controller('LocationController', function($scope,
        $ionicLoading,
        $ionicPopup,
        $ionicHistory,
        $state,
        dropzoneService,
        settingsService) {
        var getDropzoneFromService = function(position, nameFilter) {
            dropzoneService.getDropzone(position, nameFilter).then(function(dropzone) {
                $scope.locationInfo.latitude = dropzone == null ? null : dropzone.latitude;
                $scope.locationInfo.longitude = dropzone == null ? null : dropzone.longitude;
                $scope.locationInfo.dropzoneInfo = dropzone == null ? null : dropzone.address;
                $scope.locationInfo.dropzoneName = dropzone == null ? null : dropzone.name;

                $scope.disableSaveButton = false;
                $ionicLoading.hide();
            });
        };

        $scope.onCoordinatesChanged = function() {
            $scope.locationInfo.dropzoneName = null;
            $scope.locationInfo.dropzoneInfo = null;
            $scope.disableSaveButton = false;
        };

        $scope.findDropzone = function() {
            $ionicLoading.show({
                template: '<p>Loading...</p><ion-spinner></ion-spinner>'
            });

            navigator.geolocation.getCurrentPosition(
                function(position) {
                    getDropzoneFromService(position, $scope.locationInfo.dropzoneSearchPattern);
                },
                function(error) {
                    getDropzoneFromService(null, $scope.locationInfo.dropzoneSearchPattern);
                },
                { enableHighAccuracy: true, timeout: 3000 });
        };

        $scope.saveChanges = function() {
            if (!$scope.locationInfo.latitude || !$scope.locationInfo.longitude) {
                $ionicPopup.alert({
                    title: 'Save failed',
                    template: 'Location was not specified.'
                });

                return;
            }

            settingsService.saveLocationInfo($scope.locationInfo);

            $ionicHistory.nextViewOptions({
                disableBack: true
            });

            $state.transitionTo('app.weather');
        };

        $scope.$on('$ionicView.beforeEnter', function() {
            var locationInfo = settingsService.loadLocationInfo();
            if (locationInfo == null)
                locationInfo = {};
            locationInfo.dropzoneSearchPattern = '';

            $scope.locationInfo = locationInfo;
            $scope.disableSaveButton = true;
        });
    });