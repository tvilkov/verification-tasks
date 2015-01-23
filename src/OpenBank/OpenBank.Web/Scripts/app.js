; (function (window, $, undefined) {

    var app = (function () {
        var options, dataSource, dataCache = [],
            cleanThreshold = 1000 * 3600; // Milliseconds in 1 hour
        
        function log(message) {
            if (window.console && window.console.log)
                window.console.log(message);
        }

        function start(op) {
            options = op;

            dataSource = $.connection.dataTicker;

            //$.connection.hub.logging = true;

            // Client-side handler for server calls
            dataSource.client.onNewData = function (data) {
                log('[INFO] Data received: time=' + data.time + ', val=' + data.val);
                dataCache.push(data);
                render();
            };

            // Start the connection
            $.connection.hub.start()
                .done(function() {
                    getLastHourData();
                })
                .fail(function () {
                    log("[ERROR] Failed to start Signal-R connection");
                });
        }

        function getLastHourData() {
            dataSource.server.getLastHourData()
                .done(function (datas) {
                    log('[INFO] Loaded last hour data, count=' + datas.length);

                    dataCache = [];
                    $.each(datas, function (i, d) {
                        dataCache.push(d);
                    });

                    // One-point graph doesn't look nice - skip drawing it
                    if (datas.length > 1) render();
                    
                    // Schedule to reload data each hour (to get rid of stale data)
                    window.setTimeout(function () {
                        log("[INFO] Started stale data cleanup");
                        getLastHourData();
                    }, cleanThreshold);
                })
                .fail(function () {
                    log('[ERROR]: Failed to get data from server');
                });
        }

        function render() {
            var seria = {
                color: 'red',
                label: "OpenBank",
                // points: { show: true },
                lines: { show: true },
                data: $.map(dataCache, function (p) { return [[p.time, p.val]]; })
            };
            options.$view.plot([seria], {
                series: {
                    shadowSize: 0	// Drawing is faster without shadows
                },
                yaxis: { // Scale for better look 
                    min: 0,
                    max: 75
                },
                xaxis: {
                    mode: "time",
                    timeformat: "%H:%M:%S"
                }
            });
        }

        return {
            run: start
        };
    });

    window.app = app();

})(window, jQuery);