// Supported KaKu devices -> find, download en install corresponding libraries
#define unitCodeApa3      21177114  // replace with your own code
#define unitCodeActionOld 31        // replace with your own code
#define unitCodeActionNew 2210406   // replace with your own code

// Include files.
#include <Wire.h>
#include <SPI.h>                  // Ethernet shield uses SPI-interface
#include <Ethernet.h>             // Ethernet library (use Ethernet2.h for new ethernet shield v2)
#include <Servo.h>                // Servo library
#include <dht.h>
#include <DHT.h>                  // DHT11 Temperature & Humidity sensor library
#include <LiquidCrystal_I2C.h>    // I2C LCD library
#include <DS3231.h>

// Set Ethernet Shield MAC address  (check yours)
byte mac[] = { 0x40, 0x6c, 0x8f, 0x36, 0x84, 0x8a }; // Ethernet adapter shield S. Oosterhaven
int ethPort = 3300;  

EthernetServer server(ethPort);              // EthernetServer instance (listening on port <ethPort>).
Servo servo;                                 // Servo instantie
dht DHT;                                    // DHT instantie
LiquidCrystal_I2C lcd(0x27, 16, 2);          // 16 bij 2 LCD scherm instantie
DS3231  rtc(A1, A2);                                // Take a free port (check your router)

#define sensorPin    A0 //  Analog input pin (temp sensor)
float sensorValue = 0;  // default sensor waarde
bool on = false;
bool done = false;
bool showTime = false;
int startTime;
int endTime;

bool pinState = false;                   // Variable to store actual pin state
bool pinChange = false;                  // Variable to store actual pin change
int currentTime;                  
void DisplayLcd(char t = 'i');
String timeList[3];

void setup()
{
   Serial.begin(9600);
   //while (!Serial) { ; }               // Wait for serial port to connect. Needed for Leonardo only.
   delay(2000);
   Serial.println("Domotica project, CoffeeConnect Server\n");

  

    servo.attach(9);
    servo.write(0);
    rtc.begin();
    lcd.init();
    lcd.backlight();

    
   //Try to get an IP address from the DHCP server.
   if (Ethernet.begin(mac) == 0)
   {
      Serial.println("Could not obtain IP-address from DHCP -> do nothing");
      while (true){     // no point in carrying on, so do nothing forevermore; check your router
      }
   }
   
   Serial.println("Ethernetboard connected (pins 10, 11, 12, 13 and SPI)");
   Serial.println("Connect to DHCP source in local network (blinking led -> waiting for connection)");
   
   //Start the ethernet server.
   server.begin();

   // Print IP-address and led indication of server state
   Serial.print("Listening address: ");
   Serial.print(Ethernet.localIP());
   
   
   // for hardware debug: LED indication of server state: blinking = waiting for connection
   int IPnr = getIPComputerNumber(Ethernet.localIP());   // Get computernumber in local network 192.168.1.3 -> 3)
   Serial.print(" ["); Serial.print(IPnr); Serial.print("] "); 
   Serial.print("  [Testcase: telnet "); Serial.print(Ethernet.localIP()); Serial.print(" "); Serial.print(ethPort); Serial.println("]");
}

void loop()
{
    // Listen for incomming connection (app)
   EthernetClient ethernetClient = server.available();
   if (!ethernetClient) {
      DisplayLcd('i');
      delay(1000);
      return; // wait for connection and blink LED
   }

   Serial.println("Application connected");
   delay(2000);

   // Do what needs to be done while the socket is connected.
   while (ethernetClient.connected()) 
   {   
    ParseTime();
    
    if(!done && !on)
      DisplayLcd('d');
    else if(on && !done) {
      DisplayLcd('d');
      CheckDone();
    }
    else if(!on && done)
      DisplayLcd('k');

      // Execute when byte is received.
      while (ethernetClient.available())
      {  
         char inByte = ethernetClient.read();   // Get byte from the client. 
         if(inByte != NULL) 
         executeCommand(inByte);                // Wait for command to execute
         inByte = NULL;                         // Reset the read byte.
         delay(500);
         
      } 
   }
   Serial.println("Application disonnected");
}


// Implementation of (simple) protocol between app and Arduino
// Request (from app) is single char ('a', 's', 't', 'i' etc.)
// Response (to app) is 4 chars  (not all commands demand a response)
void executeCommand(char cmd)
{     
         char buf[4] = {'\0', '\0', '\0', '\0'};

         // Command protocol
         //Serial.print("["); Serial.print(cmd); Serial.print("] -> ");
         switch (cmd) {
         /*case 'a': // Report sensor value to the app  
            intToCharBuf(66.6, buf, 4);                // convert to charbuffer
            server.write(buf, 4);                             // response is always 4 chars (\n included)
            Serial.print("Sensor: "); Serial.println(buf);
            break;*/
         case 't': //Zet koffieapparaat aan/uit
            Serial.println("showtime");
            SetTime(1);
            OnOff();
            break;
         default:
            Serial.println("initialised");
         }
}

void SetTime(int minutes) {
  
  startTime = timeList[1].toInt();
  if (startTime >= (60 - minutes) && startTime != 0){
    endTime = minutes - (60 - startTime);
  }
  else{
    endTime = startTime + minutes;
  }
}

void CheckDone(){
  currentTime = timeList[1].toInt();
  Serial.println(currentTime);
  Serial.println(endTime);
  if (endTime == currentTime){
    done = true;
    startTime = -1;
    endTime = -1;
    OnOff();
    Serial.println("done");
  }
}

void ParseTime() {
  String timestring = rtc.getTimeStr();
  //Serial.println(timestring);
  
  if(timestring != "") {
    timeList[0] = (timestring.substring(0, 2));
    timeList[1] = (timestring.substring(3, 5));
    timeList[2] = (timestring.substring(6, 8));
    //Serial.println(timeList[2].toInt());
  }
}

void DisplayLcd(char t = 'i'){
  switch (t){
    case 'i':
      lcd.clear();
      lcd.setCursor(0, 0);
      lcd.print(Ethernet.localIP());
      
    break;
    case 'a':
      //laat vergaande tijd zien    
      lcd.clear();
      DHT.read11(sensorPin);
      lcd.print("Temperatuur: ");
      lcd.print(DHT.temperature);
      delay(2000);
      
    break;
    case 'k':
      lcd.clear();
      lcd.print("Koffie is klaar :D");
      lcd.setCursor(0, 1);
      DHT.read11(sensorPin);
      lcd.print("Temperatuur: ");
      lcd.print(DHT.temperature);
      delay(2000);
    
    break;
    case 'd':
    //Laat Tijd zien!!
      lcd.clear();
      DHT.read11(sensorPin);
      lcd.print("Temp: ");
      lcd.print(int(DHT.temperature));
      lcd.print((char)223);
      lcd.print("C");
      lcd.setCursor(0,1);
      lcd.print(rtc.getTimeStr());
      delay(1500);
    
    break;
  }
}

void OnOff(){
  if (!on){
    Serial.println("on");
    done = false;
    on = true;
    ServoOn();
  }
  else if(on && done){
    Serial.println("off");
    on = false;
    ServoOn();
  }
}

void ServoOn(){
  servo.write(40);
  delay(500);
  servo.write(0);
  delay(500);
}
// read value from pin pn, return value is mapped between 0 and mx-1
int readSensor(int pn, int mx)
{
  return map(analogRead(pn), 0, 1023, 0, mx-1);  
  Serial.println(analogRead(pn));
}

// Convert int <val> char buffer with length <len>
void intToCharBuf(int val, char buf[], int len)
{
   String s;
   s = String(val);                        // convert tot string
   if (s.length() == 1) s = "0" + s;       // prefix redundant "0" 
   if (s.length() == 2) s = "0" + s;  
   s = s + "\n";                           // add newline
   s.toCharArray(buf, len);                // convert string to char-buffer
}

// Convert IPAddress tot String (e.g. "192.168.1.105")
String IPAddressToString(IPAddress address)
{
    return String(address[0]) + "." + 
           String(address[1]) + "." + 
           String(address[2]) + "." + 
           String(address[3]);
}

// Returns B-class network-id: 192.168.1.3 -> 1)
int getIPClassB(IPAddress address)
{
    return address[2];
}

// Returns computernumber in local network: 192.168.1.3 -> 3)
int getIPComputerNumber(IPAddress address)
{
    return address[3];
}

// Returns computernumber in local network: 192.168.1.105 -> 5)
int getIPComputerNumberOffset(IPAddress address, int offset)
{
    return getIPComputerNumber(address) - offset;
}
