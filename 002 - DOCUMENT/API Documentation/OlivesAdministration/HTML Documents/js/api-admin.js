var apis = {
    "Controller": "Admin",
    "Actions": [
        {
            "Name": "LOGIN",
            "Description": "Administrator login.",
            "Link": "http://localhost:46170/Admin/Login",
            "Method": "POST",
            "Headers": [
				{
					"Name": "account_email",
					"Value": "Email which is used for admin registration"
				},
				{
					"Name": "account_password",
					"Value": "Password of admin account (hashed)"
				},
				
            ],
            "Request": {
                "Params": [
                    {
                        "Name": "Email",
                        "Description": "Email account which is used for registration account",
                        "Type": "string",
                        "Min": "",
                        "Max": ""
                    },
                    {
                        "Name": "Password",
                        "Description": "Account password",
                        "Type": "string",
                        "Min": "",
                        "Max": ""
                    }
                ]
            },
            "Response": {
                "Params": [
                    {
                        "Name": "HttpStatusCode : 400",
                        "Description": "Invalid request parameters.",
                        "Type": ""
                    },
                    {
                        "Name": "HttpStatusCode : 404",
                        "Description": "Account is invalid."
                    },
                    {
                        "Name": "HttpStatusCode : 200",
                        "Description": "Login is successful."
                    }
                ]
            }
        }
    ]
}