angular.module('starter.controllers')
    .controller('MapController', function ($scope, $state, $ionicLoading, settingsService, weatherService, unitsService) {
        var landingArrow;

        var findSecondPoint = function(latLng, R, angle) {
            var dx = R * Math.cos(angle);
            var dy = R * Math.sin(angle);

            var delta_longitude = dx / (111320 * Math.cos(latLng.lat()));
            var delta_latitude = dy / 110540;

            return new google.maps.LatLng(latLng.lat() + delta_latitude, latLng.lng() + delta_longitude);
        }

        var drawLandingArrow = function () {
            if ($scope.map == null)
                return;

            var locationInfo = settingsService.loadLocationInfo();
            var latLng = new google.maps.LatLng(locationInfo.latitude, locationInfo.longitude);

            if (landingArrow != null)
                landingArrow.setMap(null);
            if (weatherService.weather.groundWeather == null || weatherService.weather.groundWeather.windHeading == null)
                return;

            var lineSymbol = {
                path: google.maps.SymbolPath.FORWARD_CLOSED_ARROW
            };

            landingArrow = new google.maps.Polyline({
                strokeColor: "#FF0000",
                path: [latLng, findSecondPoint(latLng, 50, weatherService.weather.groundWeather.windHeading)],
                icons: [{
                    icon: lineSymbol,
                    offset: '100%'
                }],
                map: $scope.map
            });
        }

        var createExitAltitudeCombobox = function() {
            if ($scope.map == null)
                return;

            $scope.map.controls[google.maps.ControlPosition.TOP_CENTER].clear();

            if (weatherService.weather.windsAloftRecords == null || weatherService.weather.windsAloftRecords.length == 0)
                return;

            var altitudes = [];
            for (var i = 0; i < weatherService.weather.windsAloftRecords.length; i++) {
                var w = weatherService.weather.windsAloftRecords[i];
                altitudes.push(unitsService.convertAltitude(w.altitude, $scope.unitsSettings.altitudeUnits));
            }
            
            var controlDiv = document.createElement('div');

            var controlUI = document.createElement('div');
            controlUI.style.backgroundColor = '#fff';
            controlUI.style.border = '2px solid #fff';
            controlUI.style.cursor = 'pointer';
            controlUI.style.margin = '10px';
            controlUI.style.padding = '2px';
            controlDiv.appendChild(controlUI);

            var exitAltitudeSpan = document.createElement('span');
            exitAltitudeSpan.innerHTML = 'Exit at: ';
            controlUI.appendChild(exitAltitudeSpan);

            var comboBox = document.createElement('select');
            comboBox.style.paddingLeft = '5px';
            comboBox.style.paddingRight = '5px';
            controlUI.appendChild(comboBox);

            for (var i = 0; i < altitudes.length; i++) {
                var o = document.createElement("option");
                var t = document.createTextNode(altitudes[i]);
                o.setAttribute("value", altitudes[i]);
                o.appendChild(t);
                comboBox.appendChild(o);
            }

            $scope.map.controls[google.maps.ControlPosition.TOP_CENTER].push(controlDiv);
        }

        var createCircle = function(latLng, radius, color) {
            if ($scope.map == null)
                return null;

            var circleOptions = {
                strokeColor: "#000000",
                strokeOpacity: 0.25,
                strokeWeight: 2,
                fillColor: color,
                fillOpacity: 0.25,
                map: $scope.map,
                center: latLng,
                radius: radius
            };

            var circle = new google.maps.Circle(circleOptions);

            return circle;
        }

        var setMarker = function (latLng) {
            if ($scope.marker != null) {
                $scope.marker.setPosition(latLng);
                return;
            }

            $scope.marker = new google.maps.Marker({
                position: latLng,
                map: $scope.map,
                draggable: true
            });

            createCircle(latLng, 100, "#ff5500");
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
                    createExitAltitudeCombobox();
                    drawLandingArrow();
                },
                function onLoadNotNeeded() {
                    $scope.initializeMaps();
                    createExitAltitudeCombobox();
                    drawLandingArrow();
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
                    mapTypeIds: []
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
            $scope.unitsSettings = settingsService.loadUnitsSettings();
            $scope.loadWeather(false);
        });

        $scope.gotoLocationSettings = function () {
            $state.transitionTo('app.location');
        }
    });