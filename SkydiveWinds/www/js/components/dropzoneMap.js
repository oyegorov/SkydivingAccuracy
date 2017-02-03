angular.module('starter.controllers')
    .component('dropzoneMap', {
        template: '<div id="map" class="map" data-tap-disabled="true" ng-show="$ctrl.locationInfo">',
        bindings: {
            locationInfo: '<',
            weather: '<',
            unitsSettings: '<',
            onMarkerMoved: '&'
        },

        controller: function (unitsService, spottingService) {
            var ctrl = this;
            
            var createExitAltitudeCombobox = function () {
                if (ctrl.map == null || ctrl.unitsSettings == null)
                    return;

                ctrl.map.controls[google.maps.ControlPosition.TOP_CENTER].clear();

                if (ctrl.weather == null || ctrl.weather.windsAloftRecords == null || ctrl.weather.windsAloftRecords.length == 0)
                    return;

                var altitudes = [];
                for (var i = 0; i < ctrl.weather.windsAloftRecords.length; i++) {
                    var w = ctrl.weather.windsAloftRecords[i];
                    altitudes.push(unitsService.convertAltitude(w.altitude, ctrl.unitsSettings.altitudeUnits));
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

                ctrl.map.controls[google.maps.ControlPosition.TOP_CENTER].push(controlDiv);
            }

            var drawLandingArrow = function () {
                if (ctrl.map == null)
                    return;

                if (ctrl.landingArrow != null)
                    ctrl.landingArrow.setMap(null);

                if (ctrl.locationInfo == null)
                    return;
                var latLng = new google.maps.LatLng(ctrl.locationInfo.latitude, ctrl.locationInfo.longitude);

                if (ctrl.weather == null ||ctrl.weather.groundWeather == null || ctrl.weather.groundWeather.windHeading == null)
                    return;

                var lineSymbol = {
                    path: google.maps.SymbolPath.FORWARD_CLOSED_ARROW
                };

                var windSpeedMs = unitsService.convertWindSpeed(ctrl.weather.groundWeather.windSpeed, unitsService.WS_MPS);
                var forwardSpeed = 15 - windSpeedMs > 0 ? 15 - windSpeedMs : 1;
                var finalLegLength = 20 * forwardSpeed;

                ctrl.landingArrow = new google.maps.Polyline({
                    strokeColor: "#FFFF00",
                    path: [spottingService.offsetCoordinatesBy(latLng, finalLegLength, 180 + ctrl.weather.groundWeather.windHeading), latLng],
                    icons: [{
                        icon: lineSymbol,
                        offset: '100%'
                    }],
                    map: ctrl.map
                });
            }

            var drawSpot = function() {
                if (ctrl.map == null)
                    return;
                if (ctrl.spotCircle != null)
                    ctrl.spotCircle.setMap(null);
                if (ctrl.locationInfo == null)
                    return;

                var latLng = new google.maps.LatLng(ctrl.locationInfo.latitude, ctrl.locationInfo.longitude);
                var spotInformation = spottingService.getSpotInformation(ctrl.weather, latLng, 3000);
                if (spotInformation == null)
                    return;

                var circleOptions = {
                    strokeColor: "#000000",
                    strokeOpacity: 0.25,
                    strokeWeight: 2,
                    fillColor: "#ffff00",
                    fillOpacity: 0.25,
                    map: ctrl.map,
                    center: spotInformation.spotCenter,
                    radius: 200
                };

                ctrl.spotCircle = new google.maps.Circle(circleOptions);
            }

            var setMarker = function (latLng) {
                if (ctrl.marker != null) {
                    drawSpot();
                    drawLandingArrow();
                    return;
                }

                ctrl.marker = new google.maps.Marker({
                    position: latLng,
                    map: ctrl.map,
                    draggable: true
                });
                ctrl.marker.addListener('dragend', function onDragEnd(e) {
                    ctrl.locationInfo.latitude = this.position.lat();
                    ctrl.locationInfo.longitude = this.position.lng();
                    drawSpot();
                    drawLandingArrow();

                    if (ctrl.onMarkerMoved != null)
                        ctrl.onMarkerMoved();
                });

                drawSpot();
                drawLandingArrow();
            }

            var initMaps = function() {
                if (ctrl.locationInfo == null)
                    return;

                var latLng = new google.maps.LatLng(ctrl.locationInfo.latitude, ctrl.locationInfo.longitude);
                if (ctrl.map != null) {
                    ctrl.map.panTo(latLng);
                    setMarker(latLng);
                    drawSpot();
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

                ctrl.map = new google.maps.Map(document.getElementById("map"), mapOptions);

                setMarker(latLng);
                createExitAltitudeCombobox();
            }

            this.$onInit = function () {
                initMaps();
            };

            this.$onChanges = function(changesObj) {
                initMaps();
            }
        }
    });