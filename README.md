# Improvements if this API was on a production system:

#### Validation:
To handle validation of parameters i would implement a IActionFilterAttribute and a IDeviceValidationService. Removing this logic from the controller.

#### Authentication/Security:
To handle authentication/security a variety of technologies could be used.  BASIC AUT / JWT TOKEN / IDENTITY / API KEY /.
Having no Security would make the API vulnerable to attacks.

#### Logging: 
Create a logging service to handle all logging to a Database / File / Elastic / another logging framework

#### Exception handling: 
Move exception handling to a middleware to handle all exceptions
This middleware could also handle logging exceptions.

#### Business/Service Layer:
Move business logic to the 'Business' layer of the API, to ensure seperation of code.

#### Tests:
Unit test is crucial in making sure the API can handle all input to the various endpoints.
This could also be done with automated postman tests via fx Azure devops when after deploying.


#### Function improvements:
ShiftsController.GetAllShifts():  Make a optional DATETIME parameter 'since', to fetch all shifts since the supplied datetime.  This would increased performance and speed.

ShiftsController.GetAllShiftsForEmployee():  Make a optional DATETIME parameter 'since', to fetch all shifts for the employee since the supplied datetime.  This would increased performance and speed.

