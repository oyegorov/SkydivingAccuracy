angular.module('starter.controllers')
    .controller('MapController', function ($scope, $state, $ionicLoading, settingsService, weatherService, unitsService) {
        var landingArrow;
        var spotCircle;

        var findSecondPoint = function (latLng, d, angle) {
            var R = 6371e3;

            var angleRad = (360 - angle) * Math.PI / 180;
            var lat1 = latLng.lat() * Math.PI / 180;
            var lng1 = latLng.lng() * Math.PI / 180;
            var angDistance = d / R;

            var lat2 = Math.asin(Math.sin(lat1) * Math.cos(angDistance) + Math.cos(lat1) * Math.sin(angDistance) * Math.cos(angleRad));
            var dlon = Math.atan2(Math.sin(angleRad) * Math.sin(angDistance) * Math.cos(lat1),
                Math.cos(angDistance) - Math.sin(lat1) * Math.sin(lat2));
            var lng2 = lng1 - dlon + Math.PI % (2 * Math.PI) - Math.PI;

            return new google.maps.LatLng(lat2 * 180 / Math.PI, (lng2 * 180 / Math.PI + 540) % 360 - 180);
        }

        var findSpot = function(latLng, exitAltitude) {
            if (weatherService.weather.windsAloftRecords == null || weatherService.weather.windsAloftRecords.length == 0)
                return null;

            var driftAngle = (weatherService.weather.groundWeather.windHeading + weatherService.weather.windsAloftRecords[0].windHeading) / 2;
            
            var K = 25;
            var A = 3;
            var V = (weatherService.weather.groundWeather.windSpeed + weatherService.weather.windsAloftRecords[0].windSpeed) / 2;
            var D = K * A * V;

            var p = findSecondPoint(latLng, D, driftAngle);

            K = 3;

            var w = weatherService.weather.windsAloftRecords;
            for (var i = 1; i < w.length && w[i].altitude <= exitAltitude; i++) {
                driftAngle = (w[i].windHeading + w[i - 1].windHeading) / 2;

                A = (w[i].altitude - w[i - 1].altitude) / 1000;
                V = (w[i].windSpeed + w[i - 1].windSpeed) / 2;

                D = K * A * V;
                p = findSecondPoint(p, D, driftAngle);
            }

            D = 150;
            driftAngle = w[w.length - 1].windHeading;
            p = findSecondPoint(p, D, driftAngle);

            return p;
        }

        var drawLandingArrow = function () {
            if ($scope.map == null)
                return;

            if (landingArrow != null)
                landingArrow.setMap(null);

            var locationInfo = settingsService.loadLocationInfo();
            if (locationInfo == null)
                return;
            var latLng = new google.maps.LatLng(locationInfo.latitude, locationInfo.longitude);
            
            if (weatherService.weather.groundWeather == null || weatherService.weather.groundWeather.windHeading == null)
                return;

            var lineSymbol = {
                path: google.maps.SymbolPath.FORWARD_CLOSED_ARROW
            };

            var windSpeedMs = unitsService.convertWindSpeed(weatherService.weather.groundWeather.windSpeed,
                unitsService.WS_MPS);
            var forwardSpeed = 15 - windSpeedMs > 0 ? 15 - windSpeedMs : 1;
            var finalLegLength = 20 * forwardSpeed;

            landingArrow = new google.maps.Polyline({
                strokeColor: "#FFFF00",
                path: [findSecondPoint(latLng, finalLegLength, 180 + weatherService.weather.groundWeather.windHeading), latLng],
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

        var drawSpot = function() {
            if ($scope.map == null)
                return;
            if (spotCircle != null)
                spotCircle.setMap(null);
            
            var locationInfo = settingsService.loadLocationInfo();
            if (locationInfo == null)
                return;

            var latLng = new google.maps.LatLng(locationInfo.latitude, locationInfo.longitude);
            var spot = findSpot(latLng, 9000);
            if (spot == null)
                return;

            var circleOptions = {
                strokeColor: "#000000",
                strokeOpacity: 0.25,
                strokeWeight: 2,
                fillColor: "#ffff00",
                fillOpacity: 0.25,
                map: $scope.map,
                center: spot,
                radius: 200
            };

            spotCircle = new google.maps.Circle(circleOptions);
        }

        var setMarker = function (latLng) {
            if ($scope.marker != null) {
                $scope.marker.setPosition(latLng);
                drawSpot();
                return;
            }

            $scope.marker = new google.maps.Marker({
                position: latLng,
                map: $scope.map,
                draggable: true
            });
            $scope.marker.addListener('dragend', function onDragEnd(e) {
                var locationInfo = settingsService.loadLocationInfo();
                locationInfo.latitude = e.latLng.lat();
                locationInfo.longitude = e.latLng.lng();
                settingsService.saveLocationInfo(locationInfo);

                drawLandingArrow();
            });
            
            drawSpot();
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
                    drawSpot();
                },
                function onLoadNotNeeded() {
                    $scope.initializeMaps();
                    createExitAltitudeCombobox();
                    drawLandingArrow();
                    drawSpot();
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