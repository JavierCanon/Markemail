function onOpen() {
  var ui = SpreadsheetApp.getUi();
  ui.createMenu('SETUP')
      .addItem('CREATE CONTACT GROUPS', 'createContactGroups')
      .addItem('AUTHENTICATE APPLICATION', 'authenticationApp')
      //.addItem('RE-INITIALIZE CELLS', 'initializeCells')
      .addItem('SHOW SIDE-BAR', 'showSideBar')
      //.addSeparator()
      //.addSubMenu(ui.createMenu('Sub-menu')
      //    .addItem('Second item', 'menuItem2'))
      //.addItem('DISABLE TRIGGERS', 'disableTriggers')
      .addToUi();
  ui.createMenu('CONFIG:TRIGGERS')
      .addItem('ENABLE TRIGGERS', 'enableTriggers')
      .addItem('DISABLE TRIGGERS', 'disableTriggers')
      .addItem('TRIGGERS STATUS', 'statusTriggers')
      .addToUi();
  ui.createMenu('SEND:MANUAL')
      .addItem('EXECUTE:GLOBAL()', 'sendgroup1')
      .addItem('EXECUTE:AUTO-EMAIL1()', 'sendgroup2')
      .addItem('EXECUTE:AUTO-EMAIL2()', 'sendgroup3')
      .addItem('EXECUTE:AUTO-EMAIL3()', 'sendgroup4')
      .addToUi();
  ui.createMenu('TESTING')
      .addItem('GET EMAIL QUOTA', 'queryQuota')
        .addSeparator()
          .addSubMenu(ui.createMenu('TEST CONTACT CAPTURE')
            .addItem('GLOBAL() CONTACTS', 'group1ContactTest')
            .addItem('AUTO-EMAIL1() CONTACTS', 'group2ContactTest')
            .addItem('AUTO-EMAIL2() CONTACTS', 'group3ContactTest')
            .addItem('AUTO-EMAIL3() CONTACTS', 'group4ContactTest'))
       .addToUi();
  ui.createMenu('ARCHIVE')
      .addItem('GENERATE PDF OF SENT MESSAGE', 'messagePDF')      
      .addToUi();
}
/*************************onLoad*******************************/
//Declare Sheets
var emailSheet = SpreadsheetApp.getActiveSpreadsheet().getSheetByName('Email');
var dataSheet = SpreadsheetApp.getActiveSpreadsheet().getSheetByName('Data');
var logSheet = SpreadsheetApp.getActiveSpreadsheet().getSheetByName('Log');
var trackingSheet = SpreadsheetApp.getActiveSpreadsheet().getSheetByName('Tracking');
SpreadsheetApp.getActiveSpreadsheet().getSheetByName('Log').protect().setWarningOnly(true);
SpreadsheetApp.getActiveSpreadsheet().getSheetByName('Tracking').protect().setWarningOnly(true);
//Global Variables
var todaysDate = Utilities.formatDate(new Date(), "EST", "MM-dd-yyyy");
var currentTime = Utilities.formatDate(new Date(), "EST", "HH:mm:ss");
var userEmail = Session.getActiveUser().getEmail();
var username = extractTextBefore(userEmail,"@");
//Group1 = Global()
var group1 = emailSheet.getRange('B2').getValue();
var group1Subject = emailSheet.getRange('C2').getValue();
var group1Body = emailSheet.getRange('C4').getValue();
var group1Campaign = emailSheet.getRange('B8').getValue();
var group1Attachment1 = emailSheet.getRange('B6').getValue();
//Group2 = Auto-Email1()
var group2 = emailSheet.getRange('B10').getValue();
var group2Subject = emailSheet.getRange('C10').getValue();
var group2Body = emailSheet.getRange('C12').getValue();
var group2Campaign = emailSheet.getRange('B16').getValue();
var group2Attachment1 = emailSheet.getRange('B14').getValue();
//Group3 = Auto-Email2()
var group3 = emailSheet.getRange('B18').getValue();
var group3Subject = emailSheet.getRange('C18').getValue();
var group3Body = emailSheet.getRange('C20').getValue();
var group3Campaign = emailSheet.getRange('B24').getValue();
var group3Attachment1 = emailSheet.getRange('B22').getValue();
//Group4 = Auto-Email3()
var group4 = emailSheet.getRange('B26').getValue();
var group4Subject = emailSheet.getRange('C26').getValue();
var group4Body = emailSheet.getRange('C28').getValue();
var group4Campaign = emailSheet.getRange('B32').getValue();
var group4Attachment1 = emailSheet.getRange('B30').getValue();
/*************************Cell Validations*******************************/
var validCampaign = SpreadsheetApp.newDataValidation()
  .requireValueInRange(dataSheet.getRange('A2:A50'))
  .setAllowInvalid(false)
  .setHelpText('Please select a valid campaign')
  .build();
var enableAttachments = SpreadsheetApp.newDataValidation()
  .requireValueInList(['TRUE', 'FALSE'])
  .setAllowInvalid(false)
  .setHelpText('Please select a valid attachments indicator')
  .build();
/************************************************************************/
var grayColor = '#CACACA';
//email-sheet
emailSheet.getRange('A1')
  .setValue('INFO');
emailSheet.getRange('A2')
  .setValue('DATE:')
  .setBackground(grayColor);
emailSheet.getRange('A3')
  .setValue(todaysDate);
emailSheet.getRange('A4')
  .setValue('TIME:')
  .setBackground(grayColor);
emailSheet.getRange('A5')
  .setValue(currentTime);
emailSheet.getRange('B1')
  .setValue('GROUP NAME');
emailSheet.getRange('B5')
  .setValue('Enable Attachment(s)');

  emailSheet.getRange('B6')
   .setDataValidation(enableAttachments);
  emailSheet.getRange('B14')
   .setDataValidation(enableAttachments);
  emailSheet.getRange('B22')
   .setDataValidation(enableAttachments);
  emailSheet.getRange('B30')
   .setDataValidation(enableAttachments);

emailSheet.getRange('B7')
  .setValue('Select Campaign:');
emailSheet.getRange('B8')
  .setDataValidation(validCampaign);
emailSheet.getRange('B13')
  .setValue('Enable Attachment(s)');
emailSheet.getRange('B15')
  .setValue('Select Campaign:');
emailSheet.getRange('B16')
  .setDataValidation(validCampaign);
emailSheet.getRange('B21')
  .setValue('Enable Attachment(s)');
emailSheet.getRange('B23')
  .setValue('Select Campaign:');
emailSheet.getRange('B24')
  .setDataValidation(validCampaign);
emailSheet.getRange('B29')
  .setValue('Enable Attachment(s)');
emailSheet.getRange('B31')
  .setValue('Select Campaign:');
emailSheet.getRange('B32')
  .setDataValidation(validCampaign);
emailSheet.getRange('D1')
  .setValue('SUBJECT LINE OF THE EMAIL');
emailSheet.getRange('D9')
  .setValue('SUBJECT LINE OF THE EMAIL');
emailSheet.getRange('D17')
  .setValue('SUBJECT LINE OF THE EMAIL');
emailSheet.getRange('D25')
  .setValue('SUBJECT LINE OF THE EMAIL');
emailSheet.getRange('D3')
  .setValue('BODY OF THE EMAIL');
emailSheet.getRange('D11')
  .setValue('BODY OF THE EMAIL');
emailSheet.getRange('D19')
  .setValue('BODY OF THE EMAIL');
emailSheet.getRange('D27')
  .setValue('BODY OF THE EMAIL');
//data-sheet
dataSheet.getRange('A1')
  .setValue('CAMPAIGN')
  .setBackground(grayColor);
/************************************************************************/
function createContactGroups() {//Create the Contact Groups we're going to reference
  try {
    ContactsApp.getContactGroup(group1).getName();
    SpreadsheetApp.getUi().alert(group1 +' Contact Group ALREADY EXISTS');
  } catch (err) {
    ContactsApp.createContactGroup(group1);
    SpreadsheetApp.getUi().alert(group1 + ' Contact Group SUCCESSFULLY CREATED');
  }
  try {
    ContactsApp.getContactGroup(group2).getName();
    SpreadsheetApp.getUi().alert(group2 +' Contact group ALREADY EXISTS');
  } catch (err) {
    ContactsApp.createContactGroup(group2);
    SpreadsheetApp.getUi().alert(group2 + ' Contact Group SUCCESSFULLY CREATED');
  }
  try {
    ContactsApp.getContactGroup(group3).getName();
    SpreadsheetApp.getUi().alert(group3 + ' Contact Group ALREADY EXISTS');
  } catch (err) {
    ContactsApp.createContactGroup(group3);
    SpreadsheetApp.getUi().alert(group3 + ' Contact Group SUCCESSFULLY CREATED');
  }
  try {
    ContactsApp.getContactGroup(group4).getName();
    SpreadsheetApp.getUi().alert(group4 + ' Contact Group ALREADY EXISTS');
  } catch (err) {
    ContactsApp.createContactGroup(group4);
    SpreadsheetApp.getUi().alert(group4 + ' Contact Group SUCCESSFULLY CREATED');
  }
}

/******************************DATA VALIDATION FUNCTIONS*************************/
function fieldValidation(groupName, array) {
var passValidation = true;
  
  //validate the specifed groupName is a valid ContactsApp Group
  if(ContactsApp.getContactGroup(groupName).getName() != groupName || ContactsApp.getContactGroup(groupName).getName() == null) {
     passValidation = false;
     Logger.log('Failed fieldValidation: The contact group name is invalid or has not been specified.');
  }
  //Validate Email Quota
if(array.length > MailApp.getRemainingDailyQuota()) {
    passValidation = false;
    Logger.log('Failed fieldValidation: Insuffient Remaining Quota');
}
  //validate there are contacts in the specified group
if(array.length == 0) {
    passValidation = false;
    Logger.log('Failed fieldValidation: No Contact(s) are present in ' + groupName);
}
  
for (i = 0; i < array.length; i++) {
   //Validate Provide Email address
   var validateEmail = array[i].getPrimaryEmail();
   var emailPattern = /^[a-zA-Z0-9._-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,4}$/;
   //Validate givenName is populated
   var validateGivenName = array[i].getGivenName();
   //Validate Family Name is populated
   var validateFamilyName = array[i].getFamilyName();
   //Validate Subject Line
   switch(groupName) {
     case 'Global()':
       var validateSubject = group1Subject;
       var validateBody = group1Body;
       break;
     case 'Auto-Email1()':
       var validateSubject = group2Subject;
       var validateBody = group2Body;
       break;
     case 'Auto-Email2()':
       var validateSubject = group3Subject;
       var validateBody = group3Body;
       break;
     case 'Auto-Email3()':
       var validateSubject = group4Subject;
       var validateBody = group4Body;
       break;
     default:
       var passValidation = false;
       Logger.log('Failed fieldValidation: groupName needs to be added to Field Validation parameters!');
   }
   if (emailPattern.test(validateEmail) == false) {
      passValidation = false;
      Logger.log('Failed fieldValidation: Contact has invalid/blank e-mail address in ' + groupName);
   }
   if (validateGivenName == '' || validateGivenName == null) {
      passValidation = false;
      Logger.log('Failed fieldValidation: Contact has invalid/blank Given Name in ' + groupName);
   }
   if (validateFamilyName == '' || validateFamilyName == null) {
      passValidation = false;
      Logger.log('Failed fieldValidation: Contact has invalid/blank Family Name in ' + groupName);
   }
   if (validateSubject == '' || validateSubject == null) {
      passValidation = false;
      Logger.log('Failed fieldValidation: Email has invalid/blank Subject Field');
   }
    if (validateBody == '' || validateBody == null) {
      passValidation = false;
      Logger.log('Failed fieldValidation: Email has invalid/blank Body Field');
   }
  }
writeLog();
return passValidation
}
/******************************EMAIL EXECUTION FUNCTIONS*************************/
function sendgroup1() {//Auto-Email3()
 var contactArray = ContactsApp.getContactsByGroup(ContactsApp.getContactGroup(group1));
   if (fieldValidation(group1, contactArray) == true) {
    for (i = 0; i < contactArray.length; i++) {
     var emailBody = '<p>'
                       +'Dear '
                       + contactArray[i].getGivenName()
                       + ','
                       + '</p><p>'
                       + group1Body
                       + '</p><p></p><p>'
                       + 'Sincerely,'
                       + '<br />'
                       + '<br />'
                       + '<a href="http://www.abbottcreditsolutions.com"><img src="https://lh3.googleusercontent.com/6VT4Q9Hwzq26K1FMKwxxiq-2QHPE0R0Obxdu6034z_0_aibb3asyqx6YEChcLYqvNl615zINjhGSjFIA7tcsWt-6_IreM1J4ZhwWsjK9rx848RUmKEXpPnS9xtb8wuR6f15NKfBgHw" /</a>'
                /*
                       + 'Sandy Abbott'
                       + '<br />'
                       + 'Abbott & Associates'
                       + '<br />'
                       + '<a href="http://www.abbottcreditsolutions.com">www.abbottcreditsolutions.com</a>'
                       + '</p>'
                       
                */
                       + getTracker(group1Campaign, contactArray[i].getGivenName(), contactArray[i].getFamilyName(), group1Subject,  contactArray[i].getPrimaryEmail());
     if (group1Attachment1 !== true) {      
      GmailApp.sendEmail(contactArray[i].getPrimaryEmail(), group1Subject, group1Body, {
        htmlBody: emailBody,
          });
      } else  if (group1Attachment1 == true){
        GmailApp.sendEmail(contactArray[i].getPrimaryEmail(), group1Subject, group1Body, {
        htmlBody: emailBody,
         attachments: queryAttachments('group1')
      }
     );
    }
   }
    SpreadsheetApp.getUi().alert('Email(s) have successfully executed!');
  } else {
    GmailApp.sendEmail(Session.getEffectiveUser().getEmail(), 'Auto-Email-App: Automated Message (Failure to Execute)', 'An email-send function was unable to execute. Please refer to the log below: \r\n\r\n' + Logger.getLog());
    SpreadsheetApp.getUi().alert('Auto-Emails-App was unable to complete the operation. Please refer to the log below: \r\n\r\n' + Logger.getLog());
 }
}

function sendgroup2() {//Auto-Email3()
 var contactArray = ContactsApp.getContactsByGroup(ContactsApp.getContactGroup(group2));
   if (fieldValidation(group2, contactArray) == true) {
    for (i = 0; i < contactArray.length; i++) {
     var emailBody = '<p>'
                       +'Dear '
                       + contactArray[i].getGivenName()
                       + ','
                       + '</p><p>'
                       + group2Body
                       + '</p><p></p><p>'
                       + 'Sincerely,'
                       + '<br />'
                       + 'Sandy Abbott'
                       + '<br />'
                       + 'Abbott & Associates'
                       + '<br />'
                       + '<a href="http://www.abbottcreditsolutions.com">www.abbottcreditsolutions.com</a>'
                       + '</p>'
                       + getTracker(group2Campaign, contactArray[i].getGivenName(), contactArray[i].getFamilyName(), group2Subject, contactArray[i].getPrimaryEmail());
      if (group2Attachment1 !== true) {      
      GmailApp.sendEmail(contactArray[i].getPrimaryEmail(), group2Subject, group2Body, {
        htmlBody: emailBody,
         //attachments: [DriveApp.getFilesByName(group4Attachment1).next().getBlob()]
         });
      } else  if (group2Attachment1 == true){
        GmailApp.sendEmail(contactArray[i].getPrimaryEmail(), group2Subject, group2Body, {
        htmlBody: emailBody,
         attachments: queryAttachments('group2')
      }
     );
    }
   }
    SpreadsheetApp.getUi().alert('Email(s) have successfully executed!');
  } else {
        GmailApp.sendEmail(Session.getEffectiveUser().getEmail(), 'Auto-Email-App: Automated Message (Failure to Execute)', 'An email-send function was unable to execute. Please refer to the log below: \r\n\r\n' + Logger.getLog());
    SpreadsheetApp.getUi().alert('Auto-Emails-App was unable to complete the operation. Please refer to the log below: \r\n\r\n' + Logger.getLog());
 }
}

function sendgroup3() {//Auto-Email3()
 var contactArray = ContactsApp.getContactsByGroup(ContactsApp.getContactGroup(group3));
   if (fieldValidation(group3, contactArray) == true) {
    for (i = 0; i < contactArray.length; i++) {
     var emailBody = '<p>'
                       +'Dear '
                       + contactArray[i].getGivenName()
                       + ','
                       + '</p><p>'
                       + group3Body
                       + '</p><p></p><p>'
                       + 'Sincerely,'
                       + '<br />'
                       + 'Sandy Abbott'
                       + '<br />'
                       + 'Abbott & Associates'
                       + '<br />'
                       + '<a href="http://www.abbottcreditsolutions.com">www.abbottcreditsolutions.com</a>'
                       + '</p>'
                       + getTracker(group3Campaign, contactArray[i].getGivenName(), contactArray[i].getFamilyName(), group3Subject,  contactArray[i].getPrimaryEmail());
    if (group3Attachment1 !== true) {      
      GmailApp.sendEmail(contactArray[i].getPrimaryEmail(), group3Subject, group3Body, {
        htmlBody: emailBody,
           });
      } else  if (group3Attachment1 == true){
        GmailApp.sendEmail(contactArray[i].getPrimaryEmail(), group3Subject, group3Body, {
        htmlBody: emailBody,
         attachments: queryAttachments('group3')
      }
     );
    }
   }
    SpreadsheetApp.getUi().alert('Email(s) have successfully executed!');
  } else {
    GmailApp.sendEmail(Session.getEffectiveUser().getEmail(), 'Auto-Email-App: Automated Message (Failure to Execute)', 'An email-send function was unable to execute. Please refer to the log below: \r\n\r\n' + Logger.getLog());
    SpreadsheetApp.getUi().alert('Auto-Emails-App was unable to complete the operation. Please refer to the log below: \r\n\r\n' + Logger.getLog());
  }
}

function sendGroup4() {//Auto-Email3()
 var contactArray = ContactsApp.getContactsByGroup(ContactsApp.getContactGroup(group4));
   if (fieldValidation(group4, contactArray) == true) {
    for (i = 0; i < contactArray.length; i++) {
     var emailBody = '<p>'
                       +'Dear '
                       + contactArray[i].getGivenName()
                       + ','
                       + '</p><p>'
                       + group4Body
                       + '</p><p></p><p>'
                       + 'Sincerely,'
                       + '<br />'
                       + 'Sandy Abbott'
                       + '<br />'
                       + 'Abbott & Associates'
                       + '<br />'
                       + '<a href="http://www.abbottcreditsolutions.com">www.abbottcreditsolutions.com</a>'
                       + '</p>'
                       + getTracker(group4Campaign, contactArray[i].getGivenName(), contactArray[i].getFamilyName(), group4Subject,  contactArray[i].getPrimaryEmail());
      if (group4Attachment1 !== true) {      
      GmailApp.sendEmail(contactArray[i].getPrimaryEmail(), group4Subject, group4Body, {
        htmlBody: emailBody,
             });
      } else  if (group4Attachment1 == true){
        GmailApp.sendEmail(contactArray[i].getPrimaryEmail(), group4Subject, group4Body, {
        htmlBody: emailBody,
         attachments: queryAttachments('group4')
      }
     );
    }
   }
    SpreadsheetApp.getUi().alert('Email(s) have successfully executed!');
  } else {
    GmailApp.sendEmail(Session.getEffectiveUser().getEmail(), 'Auto-Email-App: Automated Message (Failure to Execute)', 'An email-send function was unable to execute. Please refer to the log below: \r\n\r\n' + Logger.getLog());
    SpreadsheetApp.getUi().alert('Auto-Emails-App was unable to complete the operation. Please refer to the log below: \r\n\r\n' + Logger.getLog()); 
 }
}
/******************************TESTING FUNCTIONS*********************************/
function testContactCapture(groupName) {
var contactArray = ContactsApp.getContactsByGroup(ContactsApp.getContactGroup(groupName));
var contactInfoArray = [];
var alertContacts = '';
  for (i = 0; i < contactArray.length; i++) {
    contactInfoArray.push('First Name = ' 
                          + contactArray[i].getGivenName()
                          + '\r\n'
                          + 'Last Name = '
                          + contactArray[i].getFamilyName()
                          + '\r\n'
                          + 'Email: '
                          + contactArray[i].getPrimaryEmail() 
                          + '\r\n\r\n'
                         );
    alertContacts += contactInfoArray[i];
  }
  SpreadsheetApp.getUi().alert('Pass Validation = ' 
    + fieldValidation(groupName, contactArray)
    + '\r\n'
    + 'Total # of Contacts in '
    + groupName
    + ': '
    + contactArray.length
    + '\r\n'
    + 'Total # of Email Quota remaining: '
    + MailApp.getRemainingDailyQuota()
    + '\r\n\r\n'
    + alertContacts
  );
}

function group1ContactTest() {
  testContactCapture(group1);
}

function group2ContactTest() {
  testContactCapture(group2);
}

function group3ContactTest() {
  testContactCapture(group3);
}

function group4ContactTest() {
  testContactCapture(group4);
}

/******************************MISC FUNCTIONS**************************/
function authenticationApp() {
    SpreadsheetApp.getUi() // Or DocumentApp or FormApp.
     .alert('Thank you for authenticating this application.');
}

function showSideBar() {
   SpreadsheetApp.getUi().showSidebar(HtmlService.createHtmlOutputFromFile('sidebar.html').setTitle('Auto-Email Reference & Change Log'));
}

function writeLog() {
var logEntry = Logger.getLog();
  if (logEntry != '') {
    logSheet.insertRowsAfter(1,1);
    logSheet.getRange('A2').setValue(logEntry);
  }
}

function extractTextBefore(string, character) {
  var locateCharacter = string.search(character);
  var targetText = string.slice(0, locateCharacter);
return targetText
}

function queryQuota() {
SpreadsheetApp.getUi().alert('Total Emails Remaining = ' + MailApp.getRemainingDailyQuota());
}

function queryAttachments(groupIdentifer) {
  var fileNameArray = [];
switch(groupIdentifer) {
    case 'group1':
      var files = DriveApp.getFolderById('0B5AX8tprgBH8OVZHSjNyRGYxYjQ').getFiles();
      while (files.hasNext()) {
      fileNameArray.push(DriveApp.getFileById(files.next().getId()));
     }
        break;
    case 'group2':
      var files = DriveApp.getFolderById('0B5AX8tprgBH8dkZQX2J1OXlzNmc').getFiles();
      while (files.hasNext()) {
      fileNameArray.push(DriveApp.getFileById(files.next().getId()));
     }
        break;
    case 'group3':
      var files = DriveApp.getFolderById('0B5AX8tprgBH8eU5pYWwxb3dTUkk').getFiles();
      while (files.hasNext()) {
      fileNameArray.push(DriveApp.getFileById(files.next().getId()));
     }
        break;
    case 'group4':
      var files = DriveApp.getFolderById('0B5AX8tprgBH8NmY4YjdTYm9sTFU').getFiles();
      while (files.hasNext()) {
      fileNameArray.push(DriveApp.getFileById(files.next().getId()));
     }
        break;
    default:
        break
}
  return fileNameArray
}