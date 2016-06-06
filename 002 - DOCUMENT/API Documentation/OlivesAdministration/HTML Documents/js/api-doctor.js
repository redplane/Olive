var doctors = {
    "Controller": "Doctor",
    "Actions": [
        {
            "Name": "GET_DOCTOR",
            "Description": "Get a doctor by using a specific GUID or Identity card number.",
            "Link": "http://localhost:46170/Doctor/Get",
            "Method": "GET",
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
                        "Name": "Id",
                        "Description": "Doctor GUID",
                        "Type": "string",
                        "Min": "1",
                        "Max": ""
                    },
                    {
                        "Name": "IdentityCardNo",
                        "Description": "Identity card number",
                        "Type": "string",
                        "Min": "",
                        "Max": ""
                    }
                ]
            },
            "Response": {
                "Params": [
                    {
                        "Name": "Id",
                        "Description": "Doctor GUID",
                        "Type": "string",
                        "Min": "",
                        "Max": ""
                    },
                    {
                        "Name": "LastName",
                        "Description": "Doctor last name",
                        "Type": "string",
                        "Min": "",
                        "Max": ""
                    },
                    {
                        "Name": "FirstName",
                        "Description": "Doctor first name",
                        "Type": "string",
                        "Min": "",
                        "Max": ""
                    },
                    {
                        "Name": "Birthday",
                        "Description": "Doctor birthday",
                        "Type": "long",
                        "Min": "",
                        "Max": ""
                    },
                    {
                        "Name": "Gender",
                        "Description": "Doctor gender",
                        "Type": "uint",
                        "Min": "",
                        "Max": ""
                    },
                    {
                        "Name": "Email",
                        "Description": "Doctor email",
                        "Type": "string",
                        "Min": "",
                        "Max": ""
                    },
                    {
                        "Name": "Password",
                        "Description": "Hashed password of doctor",
                        "Type": "string",
                        "Min": "",
                        "Max": ""
                    },
                    {
                        "Name": "Phone",
                        "Description": "Phone number of doctor",
                        "Type": "string",
                        "Min": "",
                        "Max": ""
                    },
                    {
                        "Name": "Money",
                        "Description": "Doctor money",
                        "Type": "double",
                        "Min": "",
                        "Max": ""
                    },
                    {
                        "Name": "Created",
                        "Description": "When the account was created",
                        "Type": "long",
                        "Min": "",
                        "Max": ""
                    },
                    {
                        "Name": "AddressLatitude",
                        "Description": "Latitude of place where doctor lives",
                        "Type": "double",
                        "Min": "",
                        "Max": ""
                    },
                    {
                        "Name": "AddressLongitude",
                        "Description": "Longitude of place where doctor lives",
                        "Type": "double",
                        "Min": "",
                        "Max": ""
                    },
                    {
                        "Name": "Role",
                        "Description": "Role of user. This value is fixed.",
                        "Type": "uint",
                        "Min": "",
                        "Max": ""
                    },
                    {
                        "Name": "Specialization",
                        "Description": "Doctor specialization",
                        "Type": "string",
                        "Min": "",
                        "Max": ""
                    },
                    {
                        "Name": "Rank",
                        "Description": "Doctor rank",
                        "Type": "double",
                        "Min": "",
                        "Max": ""
                    },
                    {
                        "Name": "IdentityCardNo",
                        "Description": "Doctor identity card number",
                        "Type": "string",
                        "Min": "",
                        "Max": ""
                    }
                ],
                "Statuses": [
                    {
                        "Name": "400",
                        "Description": "Invalid request parameters. Response with error codes will be returned with this status code.",
                        "Type": ""
                    },
                    {
                        "Name": "404",
                        "Description": "Result couldn't be found."
                    },
                    {
                        "Name": "200",
                        "Description": "Login is successful."
                    }
                ]
            }
        },
        {
            "Name": "CREATE_DOCTOR",
            "Description": "Create a doctor with specific information.",
            "Link": "http://localhost:46170/Doctor/Post",
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
                        "Name": "FirstName",
                        "Description": "Doctor first name",
                        "Type": "string",
                        "Min": "1",
                        "Max": "32"
                    },
                    {
                        "Name": "LastName",
                        "Description": "Doctor last name",
                        "Type": "string",
                        "Min": "1",
                        "Max": "32"
                    },
                    {
                        "Name": "Birthday",
                        "Description": "Doctor birthday",
                        "Type": "long",
                        "Min": "-2177452801",
                        "Max": ""
                    },
                    {
                        "Name": "Gender",
                        "Description": "Doctor gender",
                        "Type": "uint",
                        "Min": "0",
                        "Max": "1"
                    },
                    {
                        "Name": "Email",
                        "Description": "Doctor email. This is the account name of doctor.",
                        "Type": "string",
                        "Min": "1",
                        "Max": "128"
                    },
                    {
                        "Name": "Password",
                        "Description": "Password of account. This must be hashed before sending to server. (MD5)",
                        "Type": "string",
                        "Min": "1",
                        "Max": "32"
                    },
                    {
                        "Name": "Phone",
                        "Description": "Phone number doctor uses. Only contain number and spaces",
                        "Type": "string",
                        "Min": "1",
                        "Max": "15"
                    },
                    {
                        "Name": "Money",
                        "Description": "Money of account",
                        "Type": "double",
                        "Min": "1",
                        "Max": "9"
                    },
                    {
                        "Name": "Address[Latitude]",
                        "Description": "Latitude of place where doctor lives.",
                        "Type": "double",
                        "Min": "",
                        "Max": ""
                    },
                    {
                        "Name": "Address[Longitude]",
                        "Description": "Longitude of place where doctor lives.",
                        "Type": "double",
                        "Min": "",
                        "Max": ""
                    },
                    {
                        "Name": "Specialization",
                        "Description": "Doctor specialization",
                        "Type": "string",
                        "Min": "",
                        "Max": ""
                    },
                    {
                        "Name": "IdentityCardNo",
                        "Description": "Doctor identity card number (only contains numbers)",
                        "Type": "string",
                        "Min": "1",
                        "Max": "9"
                    },
                    {
                        "Name": "Status",
                        "Description": "Status of account. 0 - Disabled | 1 - Pending | 2 - Active",
                        "Type": "uint",
                        "Min": "0",
                        "Max": "2"
                    }

                ]
            },
            "Response": {
                "Params": [
                    {
                        "Name": "Id",
                        "Description": "Doctor GUID",
                        "Type": "string",
                        "Min": "",
                        "Max": ""
                    },
                    {
                        "Name": "LastName",
                        "Description": "Doctor last name",
                        "Type": "string",
                        "Min": "",
                        "Max": ""
                    },
                    {
                        "Name": "FirstName",
                        "Description": "Doctor first name",
                        "Type": "string",
                        "Min": "",
                        "Max": ""
                    },
                    {
                        "Name": "Birthday",
                        "Description": "Doctor birthday",
                        "Type": "long",
                        "Min": "",
                        "Max": ""
                    },
                    {
                        "Name": "Gender",
                        "Description": "Doctor gender",
                        "Type": "uint",
                        "Min": "",
                        "Max": ""
                    },
                    {
                        "Name": "Email",
                        "Description": "Doctor email",
                        "Type": "string",
                        "Min": "",
                        "Max": ""
                    },
                    {
                        "Name": "Password",
                        "Description": "Hashed password of doctor",
                        "Type": "string",
                        "Min": "",
                        "Max": ""
                    },
                    {
                        "Name": "Phone",
                        "Description": "Phone number of doctor",
                        "Type": "string",
                        "Min": "",
                        "Max": ""
                    },
                    {
                        "Name": "Money",
                        "Description": "Doctor money",
                        "Type": "double",
                        "Min": "",
                        "Max": ""
                    },
                    {
                        "Name": "Created",
                        "Description": "When the account was created",
                        "Type": "long",
                        "Min": "",
                        "Max": ""
                    },
                    {
                        "Name": "AddressLatitude",
                        "Description": "Latitude of place where doctor lives",
                        "Type": "double",
                        "Min": "",
                        "Max": ""
                    },
                    {
                        "Name": "AddressLongitude",
                        "Description": "Longitude of place where doctor lives",
                        "Type": "double",
                        "Min": "",
                        "Max": ""
                    },
                    {
                        "Name": "Role",
                        "Description": "Role of user. This value is fixed.",
                        "Type": "uint",
                        "Min": "",
                        "Max": ""
                    },
                    {
                        "Name": "Specialization",
                        "Description": "Doctor specialization",
                        "Type": "string",
                        "Min": "",
                        "Max": ""
                    },
                    {
                        "Name": "Rank",
                        "Description": "Doctor rank",
                        "Type": "double",
                        "Min": "",
                        "Max": ""
                    },
                    {
                        "Name": "IdentityCardNo",
                        "Description": "Doctor identity card number",
                        "Type": "string",
                        "Min": "",
                        "Max": ""
                    }
                ],
                "Statuses": [
                    {
                        "Name": "400",
                        "Description": "Invalid request parameters. Response with error codes will be returned with this status code.",
                        "Type": ""
                    },
                    {
                        "Name": "404",
                        "Description": "Result couldn't be found."
                    },
                    {
                        "Name": "200",
                        "Description": "Login is successful."
                    },
                    {
                        "Name": "409",
                        "Description": "Doctor has been created before (Perhaps, identity card has been used)"
                    }
                ]
            }
        },
        {
            "Name": "EDIT_DOCTOR",
            "Description": "Create a doctor with specific information.",
            "Link": "http://localhost:46170/Doctor/Put",
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
                        "Name": "Id",
                        "Description": "Doctor GUID",
                        "Type": "string",
                        "Min": "1",
                        "Max": "32"
                    },
                    {
                        "Name": "FirstName",
                        "Description": "Doctor first name",
                        "Type": "string",
                        "Min": "1",
                        "Max": "32"
                    },
                    {
                        "Name": "LastName",
                        "Description": "Doctor last name",
                        "Type": "string",
                        "Min": "1",
                        "Max": "32"
                    },
                    {
                        "Name": "Birthday",
                        "Description": "Doctor birthday",
                        "Type": "long",
                        "Min": "-2177452801",
                        "Max": ""
                    },
                    {
                        "Name": "Gender",
                        "Description": "Doctor gender",
                        "Type": "uint",
                        "Min": "0",
                        "Max": "1"
                    },
                    {
                        "Name": "Email",
                        "Description": "Doctor email. This is the account name of doctor.",
                        "Type": "string",
                        "Min": "1",
                        "Max": "128"
                    },
                    {
                        "Name": "Password",
                        "Description": "Password of account. This must be hashed before sending to server. (MD5)",
                        "Type": "string",
                        "Min": "1",
                        "Max": "32"
                    },
                    {
                        "Name": "Phone",
                        "Description": "Phone number doctor uses. Only contain number and spaces",
                        "Type": "string",
                        "Min": "1",
                        "Max": "15"
                    },
                    {
                        "Name": "Money",
                        "Description": "Money of account",
                        "Type": "double",
                        "Min": "",
                        "Max": ""
                    },
                    {
                        "Name": "Status",
                        "Description": "Status of account. 0 - Disabled | 1 - Pending | 2 - Active",
                        "Type": "uint",
                        "Min": "0",
                        "Max": "2"
                    },
                    {
                        "Name": "Address[Latitude]",
                        "Description": "Latitude of place where doctor lives. Leaves null if position doesn't need updating",
                        "Type": "double",
                        "Min": "",
                        "Max": ""
                    },
                    {
                        "Name": "Address[Longitude]",
                        "Description": "Longitude of place where doctor lives. Leaves null if position doesn't need updating",
                        "Type": "double",
                        "Min": "",
                        "Max": ""
                    },
                    {
                        "Name": "Specialization",
                        "Description": "Doctor specialization",
                        "Type": "string",
                        "Min": "",
                        "Max": ""
                    },
                    {
                        "Name": "IdentityCardNo",
                        "Description": "Doctor identity card number (only contains numbers)",
                        "Type": "string",
                        "Min": "1",
                        "Max": "9"
                    }
                ]
            },
            "Response": {
                "Params": [
                    {
                        "Name": "Id",
                        "Description": "Doctor GUID",
                        "Type": "string",
                        "Min": "",
                        "Max": ""
                    },
                    {
                        "Name": "LastName",
                        "Description": "Doctor last name",
                        "Type": "string",
                        "Min": "",
                        "Max": ""
                    },
                    {
                        "Name": "FirstName",
                        "Description": "Doctor first name",
                        "Type": "string",
                        "Min": "",
                        "Max": ""
                    },
                    {
                        "Name": "Birthday",
                        "Description": "Doctor birthday",
                        "Type": "long",
                        "Min": "",
                        "Max": ""
                    },
                    {
                        "Name": "Gender",
                        "Description": "Doctor gender",
                        "Type": "uint",
                        "Min": "",
                        "Max": ""
                    },
                    {
                        "Name": "Email",
                        "Description": "Doctor email",
                        "Type": "string",
                        "Min": "",
                        "Max": ""
                    },
                    {
                        "Name": "Password",
                        "Description": "Hashed password of doctor",
                        "Type": "string",
                        "Min": "",
                        "Max": ""
                    },
                    {
                        "Name": "Phone",
                        "Description": "Phone number of doctor",
                        "Type": "string",
                        "Min": "",
                        "Max": ""
                    },
                    {
                        "Name": "Money",
                        "Description": "Doctor money",
                        "Type": "double",
                        "Min": "",
                        "Max": ""
                    },
                    {
                        "Name": "Created",
                        "Description": "When the account was created",
                        "Type": "long",
                        "Min": "",
                        "Max": ""
                    },
                    {
                        "Name": "AddressLatitude",
                        "Description": "Latitude of place where doctor lives",
                        "Type": "double",
                        "Min": "",
                        "Max": ""
                    },
                    {
                        "Name": "AddressLongitude",
                        "Description": "Longitude of place where doctor lives",
                        "Type": "double",
                        "Min": "",
                        "Max": ""
                    },
                    {
                        "Name": "Specialization",
                        "Description": "Doctor specialization",
                        "Type": "string",
                        "Min": "",
                        "Max": ""
                    },
                    {
                        "Name": "Rank",
                        "Description": "Doctor rank",
                        "Type": "double",
                        "Min": "",
                        "Max": ""
                    },
                    {
                        "Name": "IdentityCardNo",
                        "Description": "Doctor identity card number",
                        "Type": "string",
                        "Min": "",
                        "Max": ""
                    }
                ],
                "Statuses": [
                    {
                        "Name": "400",
                        "Description": "Invalid request parameters. Response with error codes will be returned with this status code.",
                        "Type": ""
                    },
                    {
                        "Name": "404",
                        "Description": "Result couldn't be found."
                    },
                    {
                        "Name": "200",
                        "Description": "Login is successful."
                    },
                    {
                        "Name": "409",
                        "Description": "Identity card is in use. This status comes with an error code."
                    }
                ]
            }
        },
        {
            "Name": "FILTER_DOCTOR",
            "Description": "Create a doctor with specific information.",
            "Link": "http://localhost:46170/Doctor/Filter",
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
                        "Name": "Id",
                        "Description": "Doctor GUID",
                        "Type": "string",
                        "Min": "1",
                        "Max": "32"
                    },
                    {
                        "Name": "FirstName",
                        "Description": "Doctor first name",
                        "Type": "string",
                        "Min": "",
                        "Max": ""
                    },
                    {
                        "Name": "LastName",
                        "Description": "Doctor last name",
                        "Type": "string",
                        "Min": "",
                        "Max": ""
                    },
                    {
                        "Name": "BirthdayFrom",
                        "Description": "Time after which person was born",
                        "Type": "long",
                        "Min": "",
                        "Max": ""
                    },
                    {
                        "Name": "BirthdayTo",
                        "Description": "Time before which person had been born",
                        "Type": "long",
                        "Min": "",
                        "Max": ""
                    },
                    {
                        "Name": "Gender",
                        "Description": "Doctor gender",
                        "Type": "uint",
                        "Min": "0",
                        "Max": "1"
                    },
                    {
                        "Name": "Email",
                        "Description": "Doctor email. This is the account name of doctor.",
                        "Type": "string",
                        "Min": "1",
                        "Max": "128"
                    },
                    {
                        "Name": "Phone",
                        "Description": "Phone number doctor uses. Only contain number and spaces",
                        "Type": "string",
                        "Min": "1",
                        "Max": "15"
                    },
                    {
                        "Name": "MoneyFrom",
                        "Description": "Amount which user money must be higher",
                        "Type": "double",
                        "Min": "",
                        "Max": ""
                    },
                    {
                        "Name": "MoneyTo",
                        "Description": "Amount which user money must be lower",
                        "Type": "double",
                        "Min": "",
                        "Max": ""
                    },
                    {
                        "Name": "CreatedFrom",
                        "Description": "Time after which account was created.",
                        "Type": "long",
                        "Min": "",
                        "Max": ""
                    },
                    {
                        "Name": "CreatedTo",
                        "Description": "Time before which account had been created.",
                        "Type": "long",
                        "Min": "",
                        "Max": ""
                    },
                    {
                        "Name": "Status",
                        "Description": "Status of account. 0 - Disabled | 1 - Pending | 2 - Active",
                        "Type": "uint",
                        "Min": "0",
                        "Max": "2"
                    },
                    {
                        "Name": "Specialization",
                        "Description": "Doctor specialization",
                        "Type": "string",
                        "Min": "",
                        "Max": ""
                    },
                    {
                        "Name": "RankFrom",
                        "Description": "Rank which user's rank must be higher",
                        "Type": "double",
                        "Min": "",
                        "Max": ""
                    },
                    {
                        "Name": "RankTo",
                        "Description": "Rank which user's rank must be lower",
                        "Type": "double",
                        "Min": "",
                        "Max": ""
                    },
                    {
                        "Name": "IdentityCardNo",
                        "Description": "Doctor identity card number (only contains numbers)",
                        "Type": "string",
                        "Min": "",
                        "Max": ""
                    },
                    {
                        "Name": "Page",
                        "Description": "Page index",
                        "Type": "uint",
                        "Min": "0",
                        "Max": ""
                    },
                    {
                        "Name": "Records",
                        "Description": "Records which can be displayed in a page.",
                        "Type": "uint",
                        "Min": "1",
                        "Max": "20"
                    }
                ]
            },
            "Response": {
                "Params": [
                    {
                        "Name": "Id",
                        "Description": "Doctor GUID",
                        "Type": "string",
                        "Min": "",
                        "Max": ""
                    },
                    {
                        "Name": "LastName",
                        "Description": "Doctor last name",
                        "Type": "string",
                        "Min": "",
                        "Max": ""
                    },
                    {
                        "Name": "FirstName",
                        "Description": "Doctor first name",
                        "Type": "string",
                        "Min": "",
                        "Max": ""
                    },
                    {
                        "Name": "Birthday",
                        "Description": "Doctor birthday",
                        "Type": "long",
                        "Min": "",
                        "Max": ""
                    },
                    {
                        "Name": "Gender",
                        "Description": "Doctor gender",
                        "Type": "uint",
                        "Min": "",
                        "Max": ""
                    },
                    {
                        "Name": "Email",
                        "Description": "Doctor email",
                        "Type": "string",
                        "Min": "",
                        "Max": ""
                    },
                    {
                        "Name": "Password",
                        "Description": "Hashed password of doctor",
                        "Type": "string",
                        "Min": "",
                        "Max": ""
                    },
                    {
                        "Name": "Phone",
                        "Description": "Phone number of doctor",
                        "Type": "string",
                        "Min": "",
                        "Max": ""
                    },
                    {
                        "Name": "Money",
                        "Description": "Doctor money",
                        "Type": "double",
                        "Min": "",
                        "Max": ""
                    },
                    {
                        "Name": "Created",
                        "Description": "When the account was created",
                        "Type": "long",
                        "Min": "",
                        "Max": ""
                    },
                    {
                        "Name": "AddressLatitude",
                        "Description": "Latitude of place where doctor lives",
                        "Type": "double",
                        "Min": "",
                        "Max": ""
                    },
                    {
                        "Name": "AddressLongitude",
                        "Description": "Longitude of place where doctor lives",
                        "Type": "double",
                        "Min": "",
                        "Max": ""
                    },
                    {
                        "Name": "Specialization",
                        "Description": "Doctor specialization",
                        "Type": "string",
                        "Min": "",
                        "Max": ""
                    },
                    {
                        "Name": "Rank",
                        "Description": "Doctor rank",
                        "Type": "double",
                        "Min": "",
                        "Max": ""
                    },
                    {
                        "Name": "IdentityCardNo",
                        "Description": "Doctor identity card number",
                        "Type": "string",
                        "Min": "",
                        "Max": ""
                    }
                ],
                "Statuses": [
                    {
                        "Name": "400",
                        "Description": "Invalid request parameters. Response with error codes will be returned with this status code.",
                        "Type": ""
                    },
                    {
                        "Name": "404",
                        "Description": "Result couldn't be found."
                    },
                    {
                        "Name": "200",
                        "Description": "Login is successful."
                    },
                    {
                        "Name": "409",
                        "Description": "Identity card is in use. This status comes with an error code."
                    }
                ]
            }
        }
    ]
}