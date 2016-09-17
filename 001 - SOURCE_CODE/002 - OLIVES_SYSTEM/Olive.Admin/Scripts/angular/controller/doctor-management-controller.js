angular.module("DoctorModule")
    .controller("DoctorController",
    function ($scope) {

        // Check whether records are being loaded or not.
        $scope.isResultLoading = false;

        // List of statuses which are used for filtering purpose.
        $scope.statuses = statuses;

        // List of genders which can be used for filtering.
        $scope.genders = genders;

        // List of properties can be used for filtering doctor.
        $scope.filterAccountSort = filterAccountSort;

        // List of direction which results can be sorted as.
        $scope.sortDirections = sortDirections;

        // This view model is for filtering doctors.
        $scope.filterAccountViewModel = {};

        // List of roles which can be selected for filter purpose.
        $scope.accountRoles = accountRoles;


        // This view model is for pagination purpose.
        $scope.pagination = {
            page: 1,
            records: $scope.filterAccountViewModel.records
        }

        // Global date picker global configurations.
        $scope.globalDateTimeOptions = {
            formatYear: 'yy',
            startingDay: 1
        };

        // This object is for doctor management state.
        $scope.datePickerToggleState = {
            minCreated: false,
            maxCreated: false,
            minBirthday: false,
            maxBirthday: false,

            openMinCreated: function () {
                this.minCreated = true;
                $scope.$applyAsync();
            },
            openMaxCreated: function () {
                this.maxCreated = true;
                $scope.$applyAsync();
            },

            openMinBirthday: function () {
                this.minBirthday = true;
                $scope.$applyAsync();
            },
            openMaxBirthday: function () {
                this.maxBirthday = true;
                $scope.$applyAsync();
            }
        }

        // This function is for converting doctor account status to box class.
        $scope.convertToBoxClass = function (status) {
            if (status === 2)
                return "box-primary";

            if (status === 1)
                return "box-warning";

            return "box-danger";
        }

        // This function is for getting doctors list from service.
        $scope.getDoctors = function (page) {

            var filterAccountViewModel = jQuery.extend(true, {}, $scope.filterAccountViewModel);
            if (page == undefined)
                page = 0;
            
            // Update filter doctor view model page.
            filterAccountViewModel.page = page;
            filterAccountViewModel.minBirthday = $.findBeginningTimeOfDay($scope.filterAccountViewModel.minBirthday);
            filterAccountViewModel.maxBirthday = $.findEndingTimeOfDay($scope.filterAccountViewModel.maxBirthday);
            filterAccountViewModel.minCreated = $.findBeginningTimeOfDay($scope.filterAccountViewModel.minCreated);
            filterAccountViewModel.maxCreated = $.findEndingTimeOfDay($scope.filterAccountViewModel.maxCreated);
            filterAccountViewModel.genders = $scope.filterAccountViewModel.genders.map(function (x) { return x.value; });
            filterAccountViewModel.statuses = $scope.filterAccountViewModel.statuses
                .map(function (x) { return x.value; });
            filterAccountViewModel.roles = $scope.filterAccountViewModel.roles.map(function (x) { return x.value; });

            // Update pagination page.
            $scope.pagination.page = page + 1;

            $.ajax({
                url: "/account/filter",
                type: "post",
                data: angular.toJson(filterAccountViewModel),
                contentType: "application/json",
                beforeSend: function () {
                    $scope.isResultLoading = true;
                    $scope.$applyAsync();
                    return;
                },
                success: function (data) {
                    $scope.data = data;
                    $scope.$applyAsync();
                },
                complete: function () {
                    $scope.isResultLoading = false;
                    $scope.pagination.records = $scope.filterAccountViewModel.records;
                    $scope.$applyAsync();
                    return;
                }
            });
        }

        // This function is for reseting doctor filter view model.
        $scope.reset = function () {

            // Reset the doctor filter.
            $scope.filterAccountViewModel = {
                email: null,
                phone: null,
                minLastModified: null,
                maxLastModified: null,
                firstName: null,
                lastName: null,
                minBirthday: null,
                maxBirthday: null,
                minCreated: null,
                maxCreated: null,
                minRank: null,
                maxRank: null,
                direction: $scope.sortDirections[0].value,
                sort: $scope.filterAccountSort[0].value,
                statuses: $scope.statuses.map(function (x) {
                    return x.value;
                }),
                genders: $scope.genders.map(function(x) {
                    return x.value;
                }),
                roles: $scope.accountRoles.map(function(x) {
                    return x.value;
                }),
                city: null,
                country: null,
                page: 0,
                records: 20
            }

            // Notify another module about the property change.
            $scope.$applyAsync();
        }

        // Call the reset function to reset filter on page load.
        $scope.reset();

        // As the page is loaded, load all default doctors.
        $scope.getDoctors(0);

        // As the filter confirm button is clicked. Load all doctors for the first page.
        $scope.initialLoad = function () {

            // Reset the page
            $scope.filterAccountViewModel.page = 0;
            $scope.$applyAsync();

            // Do the load.
            $scope.getDoctors();
        }

        // This function is for checking whether date range is valid or not.
        $scope.isDateRangeValid = function (minDate, maxDate) {

            // Minimum date is not specified.
            if (minDate == null)
                return true;

            // Maximum date is not speicified.
            if (maxDate == null)
                return true;

            // Min date is smaller than max date.
            if (minDate < maxDate)
                return true;

            return false;
        }

        
    });