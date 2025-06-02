// This section is for connection credentials. We could instead keep a connection string in a separate file and run it first. It will change per environment.
username = "Your username";
password = GetPassword(); //Put in a function to keep this off screen when I demo.
database = "Events";
expectedDeleteCount = "1";
db = connect("mongodb://localhost:27017/" + database);

// Declaring this outside of actual queries because 
// 1) we need to ensure it is identical when used in multiple queries and 
// 2) we will need to change it per environment 
deleteFilter = { PolicyNumber: { $in: ["1730TID017"] } };

// Starting a session is necessary to run transactionally.
session = db.getMongo().startSession({ readPreference: { mode: "primary" } });
session.startTransaction({ readConcern: { level: "local" }, writeConcern: { w: "majority" } });


foundDeleteCount = db.Policies.find(deleteFilter).count();
print(expectedDeleteCount + " policies expected for deletion. " + foundDeleteCount + " policies found for deletion");

try {
    if (expectedDeleteCount == foundDeleteCount) {
        print("Preparing to delete " + foundDeleteCount + " policies.");
        result = db.Policies.deleteMany(deleteFilter); //To see an exception, remove the database name from this command.
        print(result.DeletedCount + " policies were deleted.");
        session.abortTransaction();
    } else {
        print("Aborting due to unexpected delete count.");
        session.abortTransaction();
    }
} catch (error) {
    print(error);
    throw error;
}

session.endSession();


















// Putting the password away from the body so as not to expose it during screen sharing.
function GetPassword() {
    return "PasswordGoesHere";
}