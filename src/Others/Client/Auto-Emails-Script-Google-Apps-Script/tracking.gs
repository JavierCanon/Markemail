function doGet(e) {
  var token = e.parameter.token;
  callTracker(token);
  return HtmlService.createHtmlOutputFromFile('webpage.html');
}

function getTracker(campaignText, givenName, familyName, subjectLine, recipientEmailAddress) {
  var decodedToken = Utilities.getUuid() + Utilities.formatDate(new Date(), "EST", "YYYYMMddhhmmssSS");
  var encodedToken = Utilities.base64EncodeWebSafe(decodedToken);
  var trackerTag = "<img src='" + ScriptApp.getService().getUrl() + "?token=" + encodedToken + "' width='1' height='1'/>";
  var trackerSheet = SpreadsheetApp.getActiveSpreadsheet().getSheetByName('Tracking');
  trackerSheet.insertRowsAfter(1,1);
  trackerSheet.getRange('A2').setValue(campaignText);
  trackerSheet.getRange('B2').setValue(givenName + ' ' + familyName);
  trackerSheet.getRange('C2').setValue(subjectLine);
  trackerSheet.getRange('D2').setValue(Utilities.formatDate(new Date(), "EST", "MM-dd-yyyy"));
  trackerSheet.getRange('F2').setValue('SENT').setBackground('#FF0000');
  trackerSheet.getRange('G2').setValue(encodedToken);
  trackerSheet.getRange('H2').setValue(decodedToken);
  trackerSheet.getRange('I2').setValue(recipientEmailAddress);
  return trackerTag
}

function callTracker(token) {
  var trackerSheetValues = SpreadsheetApp.getActiveSpreadsheet().getSheetByName('Tracking').getDataRange().getValues();
  for(i = 0; i < trackerSheetValues.length; i++){
    if(trackerSheetValues[i][6] == token) { //[6] Looking at contents of column "G"
        SpreadsheetApp.getActiveSpreadsheet().getSheetByName('Tracking').getRange(i+1, 5).setValue(Utilities.formatDate(new Date(), "EST", "MM-dd-yyyy"));
        SpreadsheetApp.getActiveSpreadsheet().getSheetByName('Tracking').getRange(i+1, 6).setValue('OPENED').setBackground('#00ff00');
    }
  }
  Logger.log('Email token has been received: ' + token);
  writeLog();
}

function messagePDF() {
  var ui = SpreadsheetApp.getUi();
  var trackerSheetValues = SpreadsheetApp.getActiveSpreadsheet().getSheetByName('Tracking').getDataRange().getValues();
  var maxTrackingRows = SpreadsheetApp.getActiveSpreadsheet().getSheetByName('Tracking').getMaxRows();
  var response = ui.prompt('Generate PDF of Sent Message', 'Please input line number of unique mail message you want to generate:', ui.ButtonSet.OK_CANCEL);
    if(response.getSelectedButton() == ui.Button.OK) {
      var responseNumber = Number(response.getResponseText());
        if (responseNumber <= maxTrackingRows && responseNumber != 1) {
          var messagePrimaryEmail = trackerSheetValues[responseNumber][8];  
          var threads = GmailApp.search('to:' + messagePrimaryEmail);
            if (threads.length > 0) {
             for (var t=0; t<threads.length; t++) {
               var rawDump = GmailApp.getMessageById(threads[t].getId()).getRawContent();
               var searchString = trackerSheetValues[responseNumber][6].slice(0,65);
              if (rawDump.match(searchString) != null) {
                  //Some code adapted from: https://ctrlq.org/code/19117-save-gmail-as-pdf?_ga=2.193981161.598292966.1512873775-1061456249.1512873774
               var msgs = threads[t].getMessages();
               var html = "";
               var attachments = [];
               var subject = threads[t].getFirstMessageSubject();
               /* Append all the threads in a message in an HTML document */
                 for (var m=0; m<msgs.length; m++) {
                   var msg = msgs[m];
                   html += "From: " + msg.getFrom() + "<br />";
                   html += "To: " + msg.getTo() + "<br />";
                   html += "Date: " + msg.getDate() + "<br />";
                   html += "Subject: " + msg.getSubject() + "<br />";
                   html += "<hr />";
                   html += msg.getBody().replace(/<img[^>]*>/g,"");
                   html += "<hr />";
                 var atts = msg.getAttachments();
                 for (var a=0; a<atts.length; a++) {
                   attachments.push(atts[a]);
                 }
               }
               /* Save the attachment files and create links in the document's footer */
               if (attachments.length > 0) {
                 var footer = "<strong>Attachments:</strong><ul>";
                 for (var z=0; z<attachments.length; z++) {
                   //var file = folder.createFile(attachments[z]);
                   footer += "<li>" + attachments[z].getName() + "</a></li>";
                 }
                 html += footer + "</ul>";
               }
               /* Convert the Email Thread into a PDF File */
               DriveApp.getFolderById('1FygxJFEeNIZgrfg1tuin9LF42yNlK8Jo').createFile("temp.html", html, "text/html").getAs("application/pdf").setName('sample');
               var tempFile = DriveApp.getFolderById('1FygxJFEeNIZgrfg1tuin9LF42yNlK8Jo').createFile("temp.html", html, "text/html");
               var outputPDF = DriveApp.getFolderById('1FygxJFEeNIZgrfg1tuin9LF42yNlK8Jo').createFile(tempFile.getAs("application/pdf")).setName(subject + ".pdf").getId();
               tempFile.setTrashed(true);
                Logger.log(outputPDF);
                var pdfInterface = HtmlService.createHtmlOutput('<p><a href=\"https://drive.google.com/open?id='+outputPDF+'\" target="_blank">https://drive.google.com/open?id='+outputPDF+'</a></p>').setWidth(800);
               ui.showModalDialog(pdfInterface, 'DOWNLOAD MESSAGE PDF');
              } //this is where the rawDump error message should go
             } 
           } else {
             ui.alert('ERROR: No Email Threads Found!');
         }
       } else {
          ui.alert('You did not enter a valid line number! Maximum possible value is: ' + SpreadsheetApp.getActiveSpreadsheet().getSheetByName('Tracking').getMaxRows());
    }
  }
}