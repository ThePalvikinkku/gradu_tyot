*** Settings ***
Documentation     Actual test cases. CommonFunctions.robot and Setup.robot are imported here to be used in the test cases.
Library           Browser
Library           String
Resource          CommonFunctions.robot
Resource          Setup.robot

Test Setup       Setup Browser
Test Teardown    Teardown Browser

*** Test Cases ***

Buy a flat white
    Click Coffee Item    Flat_White
    Click checkout button
    Fill in checkout form    Niko Leppanen    thesis@gradu.fi
    Submit checkout form
    Check that purchase is successful

Add Cafe Latte to cart and then add one more from cart
    Click Coffee Item    Cafe_Latte
    Go to cart page
    Add one more coffee from cart    Cafe Latte
    Check amount of coffee in cart    Cafe Latte    2

Add three cappucinos, apply discount, check the discount is correct
    Click Coffee Item    Cappuccino
    Click Coffee Item    Cappuccino
    Click Coffee Item    Cappuccino
    Accept Promotion Coffee
    Go to cart page
    Check that discounted price is correct   (Discounted) Mocha    4.00

Broken test case example
    Click Coffee Item    Espresso
    Go to cart page
    Check amount of coffee in cart    Espresso    2
    