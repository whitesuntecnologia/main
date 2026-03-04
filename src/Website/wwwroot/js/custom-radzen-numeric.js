'use strict';

let inputs = [];

export function setUnsigned(id, value) {

    var ret = false;
    const input = inputs.find(x => x.id === id);
    if (input != undefined) {
        input.unsigned = value;
        ret = true;
    }
    return ret;
}

export function setMaxLength(id, value) {
    const inputConfiguration = inputs.find(x => x.id === id);

    if (inputConfiguration != undefined> 0) {
        const { input } = inputConfiguration;

        if (!inputConfiguration) {
            return false;
        }

        inputConfiguration.maxLength = value;

        input.removeAttribute("maxlength");
        input.setAttribute("maxlength", inputConfiguration.maxLength);
    }
    return true;
}

export function setMaxIntegers(id, value) {

    var ret = false;
    const input = inputs.find(x => x.id === id);
    if (input != undefined) {
        
        input.maxIntegers = value;
        ret = true;
    }
    return ret;
}

export function setAllowsDecimals(id, value) {
    var ret = false;
    const input = inputs.find(x => x.id === id);
    if (input != undefined) {
        input.allowsDecimals = value;
        ret = true;
    }
    return ret;
}

export function setMaxDecimals(id, value) {
    var ret = false;
    const input = inputs.find(x => x.id === id);
    if (input != undefined) {
        input.maxDecimals = value;
        ret = true;
    }
    return ret;
}

export function setMaxValue(id, value) {
    const input = inputs.find(x => x.id === id);
    input.vMax = value;
    return true;
}

export function initializeInput(id, allowsDecimals) {
    const inputSearched = inputs.find(x => x.id === id);

    if (inputSearched) {
        inputs = inputs.filter(x => x.id !== inputSearched.id);
    }

    const input = $(`#${id} > input`)[0];

    if (!input) {
        return false;
    }
    const inputKeydownListener = function (e) {

        if (e.key == 'Delete' || e.key == 'Backspace') {
            inputKeypressListener(e);
        }
        
        return;
    }

    const inputKeypressListener = function (e) {
        
        const inputConfiguration = inputs.find(x => x.id === id);
        const { input, maxIntegers, maxDecimals } = inputConfiguration;
        var value = '';
        var commaPosition = 0;
        var parteDecimal = false;
        var parteEntera = false;

        var reg = new RegExp('^[0-9,.-]$');
        

        if (maxIntegers <= 0) {
            e.preventDefault();
            return;
        }

        if (e.key != 'Delete' && e.key != 'Backspace' && !reg.test(e.key)) {
            e.preventDefault();
            return;
        }

        if (inputConfiguration.unsigned && e.key === '-') {
            e.preventDefault();
            return;
        }
        
        if (e.key == 'Delete') {
            if (input.selectionStart === input.selectionEnd) {
                value = input.value.substring(0, input.selectionStart) + input.value.substring(input.selectionEnd + 1, input.value.length);
            }
            else {
                value = input.value.substring(0, input.selectionStart ) + input.value.substring(input.selectionEnd, input.value.length);
            }
        }
        else if (e.key == 'Backspace') {
            if (input.selectionStart === input.selectionEnd) {
                value = input.value.substring(0, input.selectionStart - 1) + input.value.substring(input.selectionEnd, input.value.length);
            }
            else {
                value = input.value.substring(0, input.selectionStart) + input.value.substring(input.selectionEnd, input.value.length);
            }
        }
        else {
            //value = (input.selectionStart === input.selectionEnd ? input.value : input.value.substring(0, input.selectionStart) + input.value.substring(input.selectionEnd, input.value.length));
            value = input.value.substring(0, input.selectionStart) + e.key + input.value.substring(input.selectionEnd, input.value.length);
        }
        if (e.key === '-') {
            if (inputConfiguration.unsigned) {
                e.preventDefault();
                return;
            }
            if (countChar(value, "-") > 1) {
                e.preventDefault();
                return;
            }
            if (value.substring(0, 1) != '-') {
                e.preventDefault()
                return;
            }
        }

        if ((e.key === ',' || e.key === '.') && inputConfiguration.allowsDecimals) {
            if (value.length === 1) {
                e.preventDefault();
                return;
            }
            else if (countDecimalSeparator(value) > 1) {
                e.preventDefault();
                return;
            }
            else {
                value = input.value.substring(0, input.selectionStart) + ',' + input.value.substring(input.selectionEnd, input.value.length);
            }
        }
        else if ((e.key === ',' || e.key === '.') && !inputConfiguration.allowsDecimals) {
            e.preventDefault();
            return;
        }

        commaPosition = value.indexOf(',');
        parteDecimal = (commaPosition !== -1 && value.substring(commaPosition, value.length-1).length > 0);
        if (commaPosition !== -1) {
            parteEntera = (value.substring(0, commaPosition).length > 0);
        }
        else {
            parteEntera = (value.length > 0);
        }
        
        if (parteDecimal && inputConfiguration.maxDecimals > 0 && inputConfiguration.allowsDecimals ) {
            
            if (commaPosition !== -1 ) {
                if (value.substring(commaPosition + 1, value.length).length > maxDecimals) {
                    e.preventDefault();
                    return;
                }
            }
        }

        if (parteEntera && inputConfiguration.maxIntegers > 0) {
            if (commaPosition != -1 && inputConfiguration.allowsDecimals ) {
                if (value.substring(0, commaPosition).length > maxIntegers) {
                    e.preventDefault();
                    return;
                }
            }
            else {
                // 8 backspace - 46 delete
                if (e.key == 'Delete' || e.key == 'Backspace' ) {
                    if (value.length > maxIntegers) {
                        e.preventDefault();
                        return;
                    }
                }
                else if (value.length > maxIntegers) {
                    e.preventDefault();
                    return;
                }
            }

            if (e.key === '.') {
                e.preventDefault();
                var selStartAnt = input.selectionStart;
                input.value = value;
                input.selectionStart = selStartAnt + 1;
                input.selectionEnd = selStartAnt + 1;
            }
            return;
        }


      
    }
    


    input.addEventListener("keypress", inputKeypressListener);
    input.addEventListener("keydown", inputKeydownListener);
    
    input.addEventListener('change', (event) => {
        const inputConfiguration = inputs.find(x => x.id === id);
        const { maxDecimals, vMax } = inputConfiguration;
        
        setTimeout(() => {
            var reg = new RegExp('^[0-9]*(,)$');

            if (vMax !== null && input.value.replace(',', '.') > vMax) {
                input.value = vMax;
            }

            if (reg.test(input.value)) {
                input.value = `${input.value}${zerosTimes(maxDecimals)}`;
                return;
            }

            if (!input.value.includes(",") && !input.value.includes(".") && input.value.length > 0
                && inputConfiguration.maxDecimals > 0) {
                input.value = `${input.value},${zerosTimes(maxDecimals)}`;
            }

            if (input.value.includes(",") || input.value.includes(".")) {
                //input.value = input.value.replace(".", ",");
            }

        }, 100);
        
    });

    inputs.push({
        id: id,
        input: input,
        maxIntegers: 0,
        maxDecimals: 0,
        maxLength: 0,
        unsigned: false,
        vMin: null,
        vMax: null,
        allowsDecimals: allowsDecimals,
    });

    return true;
}

function zerosTimes(times) {
    var zeros = "";
    for (let i = 0; i < times; i++) {
        zeros += "0";
    }
    return zeros;
}
function countDecimalSeparator(cadena) {

    var cantidad = 0;
    for (var i = 0; i < cadena.length; i++) {
        var input = cadena[i];
        if (input === '.' || input === ',') {
            cantidad++;
        }
    }
    return cantidad;
}
function countChar(cadena,char) {

    var cantidad = 0;
    for (var i = 0; i < cadena.length; i++) {
        var input = cadena[i];
        if (input === char ) {
            cantidad++;
        }
    }
    return cantidad;
}
