// change these to connect to remotely deployed service
var serviceUrl = "../api/Processes";
var signalrUrl = "../signalr";

var topProcessApp = angular.module('topProcessApp', ['ngGrid']);

topProcessApp.controller('MainController', function ($scope, $http) {
    $scope.loading = true;
    $scope.processFilter = '';
    $scope.autoRefresh = true;

    $scope.loadBranches = function () {
        $http.get(serviceUrl)
            .success(function (data) {
                $scope.error = "";
                $scope.loading = false;
                $scope.processes = data;
                setTimeout(function () {$(document).resize();}, 400); // fix grid layout
            })
            .error(function (err) {
                $scope.loading = false;
                $scope.error = "Failed to load process info: " + err;
            });
    };

    $scope.refreshInfo = function () {
        if ($scope.autoRefresh) {
            $scope.loadBranches();
        }
        setTimeout($scope.refreshInfo, 2000);
    }

    $scope.processesGrid = {
        data: 'processes.Processes',
        enableColumnResize: true,
        enableRowSelection: false,
        columnDefs: [
            { field: 'Name', displayName: 'Name'},
            { field: 'Id', displayName: 'PID'},
            { field: 'CpuUsage', displayName: 'CPU, %', cellTemplate: '<div class="ngCellText colt{{$index}}">{{(row.getProperty(col.field)*100).toFixed(1)}}</div>' },
            { field: 'WorkingSet', displayName: 'Memory, MB', cellTemplate: '<div class="ngCellText colt{{$index}}">{{(row.getProperty(col.field)/1024).toFixed(1)}}</div>' }
        ]
    };

    $scope.refreshInfo();

    jQuery.getScript(signalrUrl + "/hubs", function () {
        // Set up SignalR
        $.connection.hub.url = signalrUrl;

        // Declare a proxy to reference the hub. 
        var hub = $.connection.notificationHub;

        // Create a function that the hub can call to broadcast messages.
        hub.client.broadcastMessage = function (message) {
            $scope.notification = message + " (" + new Date().toLocaleTimeString() + ")";
        };

        $.connection.hub.start();
    });

    $scope.dismissNotification = function() {
        $scope.notification = false;
    }
});