-- Use Database
USE TypingTestDB;
GO

-- Create TypingTests Table
CREATE TABLE [dbo].[TypingTests]
(
    [TestId] INT NOT NULL PRIMARY KEY IDENTITY(1,1),
    [UserName] NVARCHAR(100) NOT NULL,
    [TestDate] DATETIME NOT NULL DEFAULT GETDATE(),
    [WPM] INT NOT NULL,
    [Accuracy] DECIMAL(5, 2) NOT NULL,
    [ErrorCount] INT NOT NULL,
    [TimeDuration] INT NOT NULL,
    [TestText] NVARCHAR(MAX) NOT NULL,
    [TypedText] NVARCHAR(MAX) NOT NULL
);
GO

-- Sample Data (Optional)
-- INSERT INTO TypingTests (UserName, WPM, Accuracy, ErrorCount, TimeDuration, TestText, TypedText)
-- VALUES ('Sample User', 45, 96.50, 5, 60, 'Sample text here', 'Sampl text here');
-- GO

PRINT 'Database table created successfully!';
GO
