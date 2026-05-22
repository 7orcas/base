
Controllers and Services are tested separately.
Controller tests are mocked (use AI to create and update - see below).
All tests need to extend BackendTest to obtain necessary configuration and setup.


# Create controller unit tests via AI using the following prompts:
Create unit tests for all controllers in the Backend project and add them to the BackendTest project.
Use the following instructions:
- If a controller test already exists then skip it, otherwise create a new test class for the controller
- Put the new tests in directories that replicate the Backend project
- Name the test classes the same as the controller they are testing, but with 'Tests' appended to the end (e.g. if the controller is named 'UserController', the test class should be named 'UserControllerTests')
- The Backend project has the Rest APIs to be tested, they are named with the 'Controller' convention 
- Use MSTest framework
- The new tests must extend BackendTest.Base.BaseTest
- Add using GC = Backend.GlobalConstants; to the top of each test class and use GC instead of Backend.GlobalConstants in the tests
- Add using GCT = BackendTest.GlobalConstants; to the top of each test class and use GCT instead of BackendTest.GlobalConstants in the tests
- The new tests must only include the Backend project controller public REST methods - add a return placeholder for each method test: Assert.IsTrue(true);
I confirm you may read the controller files directly (Backend/Base/*Controller.cs and Backend/App/*Controller.cs)





