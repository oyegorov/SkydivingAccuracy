angular.module('starter.controllers')
    .controller('MapController', function ($scope, $state, settingsService) {
        $scope.initializeMaps = function () {
            var locationInfo = settingsService.loadLocationInfo();

            if (locationInfo == null) {
                $scope.locationPresent = false;
                $scope.dropzoneName = null;
                return;
            }

            $scope.locationPresent = true;
            if (locationInfo.dropzoneName != null)
                $scope.locationName = locationInfo.dropzoneName;
            else
                $scope.locationName = '(' + locationInfo.latitude + '; ' + locationInfo.longitude + ')';

            var latLng = new google.maps.LatLng(locationInfo.latitude, locationInfo.longitude);
            if ($scope.map != null) {
                $scope.map.panTo(latLng);
                return;
            }

            var mapOptions = {
                center: latLng,
                zoom: 15,
                mapTypeId: google.maps.MapTypeId.SATELLITE,
                mapTypeControlOptions: {
                    mapTypeIds: [google.maps.MapTypeId.SATELLITE]
                },
                disableDefaultUI: true,
                mapTypeControl: true,
                scaleControl: true,
                zoomControl: true,
                zoomControlOptions: {
                    style: google.maps.ZoomControlStyle.LARGE
                }
            };

            $scope.map = new google.maps.Map(document.getElementById("map"), mapOptions);
        }

        $scope.$on('$ionicView.beforeEnter', function () {
            $scope.initializeMaps();
        });

        $scope.gotoLocationSettings = function () {
            $state.transitionTo('app.location');
        }
    });