*** Settings ***
Documentation     Setup file for Robot Framework tests using the Browser library; mostly for initializing the browser and teardown.
Library           Browser

*** Variables ***
${BROWSER}        chromium
${HEADLESS}       True
${URL}            https://coffee-cart.app/

*** Keywords ***

Setup Browser
    [Documentation]    Opens the browser and navigates to the specified URL.
    New Browser    ${BROWSER}    headless=${HEADLESS}
    New Context
    New Page    ${URL}

Teardown Browser
    [Documentation]    Closes the browser after tests are done.    
    Run Keyword If Test Failed    Take Screenshot
    Browser.Close Browser