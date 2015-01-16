var topProcessApp = angular.module('topProcessApp', ['ngGrid', 'highcharts-ng']);

topProcessApp.controller('MainController', function ($scope, $http) {
    $scope.loading = true;
    $scope.processFilter = '';

    $scope.LoadBranches = function () {
        $http.get("../api/Processes")
            .success(function (data) {
                $scope.error = "";
                $scope.loading = false;

                $scope.processes = data;
                $scope.memoryChart.series[0].data.push(data.MemoryUsed);
                $scope.memoryChart.yAxis.max = data.MemoryTotal;

                setTimeout(function () {
                    $(document).resize();
                }, 300); // fix grid columns
            })
            .error(function (err) {
                $scope.loading = false;
                $scope.error = "Failed to load processes";
                $scope.errorDetails = err;
            });
    };

    $scope.processesGrid = {
        data: 'processes.Processes',
        enableColumnResize: true,
        enableRowSelection: true,
        //plugins: [new ngGridFlexibleHeightPlugin()]
    };

    $scope.memoryChart = {
        chart: {
            type: 'area'
        },
        title: {
            text: 'Memory Usage'
        },
        xAxis: {
            allowDecimals: false,
            labels: {
                formatter: function() {
                    return this.value;
                }
            }
        },
        yAxis: {
            title: {
                text: 'Used Memory'
            },
            labels: {
                formatter: function() {
                    return this.value/1024/1024/1024 + 'GB';
                }
            },
        },
        //options: {
        //    yAxis: [
        //        {
        //            min: 0,
        //            max: 1024 * 1024 * 1024
        //        }
        //    ],
        //},
        tooltip: {
            pointFormat: '{point.y/1024/1024} MB'
        },
        plotOptions: {
            area: {
                marker: {
                    enabled: false,
                    symbol: 'circle',
                    radius: 2,
                    states: {
                        hover: {
                            enabled: true
                        }
                    }
                }
            }
        },
        series: [
            {
                name: 'Used Memory',
                data: [100,200,300,1204*1024*1024]
            }
        ]
    };

    $scope.LoadBranches();
});