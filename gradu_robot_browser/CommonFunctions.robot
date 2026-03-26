*** Settings ***
Documentation     Locators and core functions which will be used in Tests.robot; this file is imported in the test suite file and contains keywords that can be reused across multiple test cases.
Library           Browser
Library           String


*** Keywords ***

# Coffee-cart page
Click Coffee Item
    [Arguments]    ${name}
    Click    css=[data-test="${name}"]

Accept Promotion Coffee
    Click    role=button[name="Yes, of course!"]

Decline Promotion Coffee
    Click    role=button[name="Nawh, I'll skip."]

Go to cart page
    Click    role=link[name="Cart page"]

Click checkout button
    Click    css=button.pay

Check that purchase is successful
    Wait For Elements State    text=Thanks for your purchase    visible    timeout=5s

#Checkout page

Fill in checkout form
    [Arguments]    ${name}    ${email}
    Fill Text    id=name    ${name}
    Fill Text    id=email    ${email}

Checkbox promotion email
    Click    id=promotion

Submit checkout form
    Click    role=button[name="Submit"]

# Cart page

Add one more coffee from cart
    [Arguments]    ${name}
    Click    role=button[name="Add one ${name}"]

Remove one coffee from cart
    [Arguments]    ${name}
    Click    role=button[name="Remove one ${name}"]

Delete coffee from cart
    [Arguments]    ${name}
    Click    role=button[name="Remove all ${name}"]

Check amount of coffee in cart
    [Arguments]    ${coffeeName}    ${amount}
    ${row}=    Get Element    css=li.list-item:has-text("${coffeeName}") >> nth=1
    ${desc}=   Get Element    ${row} >> css=.unit-desc:has-text("$")
    ${text}=   Get Text    ${desc}
    ${count}=   Get Regexp Matches    ${text}    x\\s*(\\d+)    1
    Should Be Equal As Integers      ${count[0]}    ${amount}

Check that discounted price is correct
    [Arguments]    ${coffeeName}    ${discountedPrice}
    ${row}=     Get Element    css=li.list-item:has-text("${coffeeName}")>>nth=1
    ${desc}=    Get Element    ${row}>>css=.unit-desc:has-text("$")
    ${text}=    Get Text       ${desc}
    ${price}=   Get Regexp Matches    ${text}    \\$(\\d+\\.\\d{2})    1
    Should Be Equal As Numbers    ${price[0]}    ${discountedPrice}