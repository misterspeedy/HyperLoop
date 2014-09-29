
const int minButton = 3;
const int maxButton = 9;

int buttonStates[maxButton-minButton+1];

void setup() {
  for (int i=minButton; i<=maxButton; i++) {
    pinMode(i, INPUT); 
    buttonStates[i-minButton] = LOW;
  } 
  Serial.begin(9600);  
}

int checkButton(int pinNo) {
  int buttonState = digitalRead(pinNo);
  return (buttonState); 
}

void loop() {
  
  for (int i=minButton; i<=maxButton; i++) {
    int state = checkButton(i);
    int stateIndex = i-minButton;
    if (state != buttonStates[stateIndex]) {
      if (state == HIGH) {
        Serial.print("D"); Serial.println(stateIndex);
      }
      else {
        Serial.print("U"); Serial.println(stateIndex);
      }
      buttonStates[stateIndex] = state;
    } 
  }
}
