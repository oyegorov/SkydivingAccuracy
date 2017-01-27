angular.module('starter.controllers')
    .controller('MapController', function ($scope, $state, $ionicLoading, settingsService, weatherService) {
        var setMarker = function(latLng) {
            if ($scope.marker != null) {
                $scope.marker.setPosition(latLng);
                return;
            }

            $scope.marker = new google.maps.Marker({
                position: latLng,
                map: $scope.map,
                draggable: true
            });
        }

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
                    $scope.initializeMaps();
                }
            );
        }

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
                setMarker(latLng);
                return;
            }

            var mapOptions = {
                center: latLng,
                zoom: 17,
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
            setMarker(latLng);
        }

        $scope.$on('$ionicView.beforeEnter', function () {
            $scope.loadWeather(false);
            $scope.initializeMaps();
        });

        $scope.gotoLocationSettings = function () {
            $state.transitionTo('app.location');
        }
    });