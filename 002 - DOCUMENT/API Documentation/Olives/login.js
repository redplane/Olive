$.ajax({
	url: 'http://oliveshealth.azurewebsites.net/api/account/login',
	method: 'post',
	contentType: 'application/json',
	data:{
		Email: 'linhdse03179@fpt.edu.vn',
		Password: 'PatientPassword79'
	}
	success: function(data){
		Status: 0,
		Id: "",
		LastName: "PatientLastName[79]",
		FirstName: "PatientFirstName[79]",
		Birthday: 636008524081388151,
		Gender: 0,
		Email: "linhdse03179@fpt.edu.vn",
		Password: "",
		Phone: "0123456779",
		Money: 79,
		Created: 636008524081388151,
		AddressLatitude: 79,
		AddressLongitude: 79,
		Role": 2
	}
})