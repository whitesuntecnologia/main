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


export function setMaxIntegers(id, value) {

    var ret = false;
    const input = inputs.find(x => x.id === id);
    if (input != undefined) {

        input.maxIntegers = value;
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

export function initializeInput(id) {
    const inputSearched = inputs.find(x => x.id === id);


    //inputSearched

    if (inputSearched) {
        inputs = inputs.filter(x => x.id !== inputSearched.id);
    }

    //const input = $(`#${id} > input`)[0];
    const input = $(`[_bl_${id}]`)[0];

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
        const { input, maxIntegers, maxDecimals, vMax } = inputConfiguration;
        var value = '';
        var commaPosition = 0;
        var tieneParteDecimal = false;
        var tieneParteEntera = false;
        var parteDecimal = "";
        var parteEntera = "";


        var reg = new RegExp('^[0-9,.-]$');
        var pesosConverter = Intl.NumberFormat("es-AR");

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
                value = input.value.substring(0, input.selectionStart) + input.value.substring(input.selectionEnd, input.value.length);
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

        if ((e.key === ',' || e.key === '.') && inputConfiguration.maxDecimals > 0) {
            if (value.length === 1) {
                e.preventDefault();
                return;
            }
            else if (countDecimalSeparator(value) > 1 || (countDecimalSeparator(value) == 1 && e.key === '.')) {
                e.preventDefault();
                return;
            }
            else {
                value = input.value.substring(0, input.selectionStart) + ',' + input.value.substring(input.selectionEnd, input.value.length);
                value = value.replaceAll(".", "");

            }
        }
        else if ((e.key === ',' || e.key === '.') && inputConfiguration.maxDecimals == 0) {
            e.preventDefault();
            return;
        }
        else if (e.key != ',' && e.key != '.') {
            value = value.replaceAll(".", "");
        }

        commaPosition = value.indexOf(',');
        parteDecimal = (commaPosition !== -1 ? value.substring(commaPosition + 1, value.length)  : "");
        parteEntera = (commaPosition !== -1 ? value.substring(0, commaPosition) : value);
        tieneParteDecimal = (parteDecimal.length > 0);
        tieneParteEntera = (parteEntera.length > 0);

        
        if (tieneParteDecimal && inputConfiguration.maxDecimals > 0 ) {

            if (commaPosition !== -1) {
                if (value.substring(commaPosition + 1, value.length).length > inputConfiguration.maxDecimals) {
                    e.preventDefault();
                    return;
                }
            }
        }

        if (tieneParteEntera && inputConfiguration.maxIntegers > 0) {

            if (vMax !== null && parseFloat(ConvertStringToNumber(value)) > vMax) {
                e.preventDefault();
                return;
            }
                
            if (commaPosition != -1 && inputConfiguration.maxDecimals > 0) {
                if (value.substring(0, commaPosition).length > maxIntegers) {
                    e.preventDefault();
                    return;
                }
            }
            else {
                // 8 backspace - 46 delete
                if (e.key == 'Delete' || e.key == 'Backspace') {
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

            var lastPosition = (input.selectionEnd == input.value.length);
            // si ES la ultima posicion y lo que escribio es un punto o coma, entonces entra.
            if (((e.key == ',' || e.key == ".") && lastPosition)) {
                e.preventDefault();
                var selStartAnt = input.selectionStart;
                input.value = pesosConverter.format(value.replaceAll(",", ".")) + ",";
                input.selectionStart = selStartAnt + 1;
                input.selectionEnd = selStartAnt + 1;
                input.dispatchEvent(new Event("input", {
                    bubbles: true,
                    cancelable: true
                }));
            }
            else {
                e.preventDefault();
                var cantEspaciosaSumar = reg.test(e.key) ? 1 : (e.key == 'Backspace' ? -1 : 0);
                var selStartAnt = input.selectionStart;
                var isSelectall = (input.selectionStart === 0 && input.selectionEnd === input.value.length);
                var cantSepMilesAnt = countChar(input.value, '.');
                var value2 = pesosConverter.format(parteEntera.replaceAll(",", "."));
                if (tieneParteDecimal)
                    value2 += "," + parteDecimal;

                var cantSepMilesAct = countChar(value2, '.');
                input.value = value2;
                if (isSelectall)
                    input.selectionStart = value2.length;
                else
                    input.selectionStart = selStartAnt + cantSepMilesAct - cantSepMilesAnt + cantEspaciosaSumar;

                input.selectionEnd = input.selectionStart;
                input.dispatchEvent(new Event("input", {
                    bubbles: true,
                    cancelable: true
                }));
            }
            return;
        }
        else {
            input.dispatchEvent(new Event("input", {
                bubbles: true,
                cancelable: true
            }))
        }

    }

    const inputBlurListener = function (e) {
        var pesosConverter = Intl.NumberFormat("es-AR");
        var value = input.value.replaceAll(".", "").replaceAll(",", ".");
        if (value.length > 0) {
            input.value = pesosConverter.format(value);
        }
        else {
            input.value = "";
        }

    }    

    const inputPasteListener = function (e) {
        e.preventDefault();
        const inputConfiguration = inputs.find(x => x.id === id);
        const { input, maxIntegers, maxDecimals, vMax } = inputConfiguration;

        var clipboardData = event.clipboardData || window.clipboardData;
        var pastedData = clipboardData.getData('text');

        var pesosConverter = Intl.NumberFormat("es-AR");
        var value = pastedData.replaceAll(".", "").replaceAll(",", ".");
        var decValue = parseFloat(ConvertStringToNumber(value));
        //lo que se está pegando es menor al máximo
        if (vMax === null || decValue <= vMax) {
            input.value = pesosConverter.format(value);
            input.dispatchEvent(new Event("input", {
                bubbles: true,
                cancelable: true
            }));

        }
    }

    input.addEventListener("keypress", inputKeypressListener);
    //input.addEventListener("keydown", inputKeydownListener);
    input.addEventListener("blur", inputBlurListener);
    input.addEventListener("paste", inputPasteListener);
    

    inputs.push({
        id: id,
        input: input,
        maxIntegers: 0,
        maxDecimals: 0,
        unsigned: false,
        vMin: null,
        vMax: null,
        allowsDecimals: true,
    });

    return true;
}

function countDecimalSeparator(cadena) {

    var cantidad = 0;
    for (var i = 0; i < cadena.length; i++) {
        var input = cadena[i];
        if (input === ',') {
            cantidad++;
        }
    }
    return cantidad;
}
function countChar(cadena, char) {

    var cantidad = 0;
    for (var i = 0; i < cadena.length; i++) {
        var input = cadena[i];
        if (input === char) {
            cantidad++;
        }
    }
    return cantidad;
}

function GetNumbers(cadena) {
    const caracterespermitidos = "1234567890.,";
    var result = "";
    for (let i = 0; i < cadena.length; i++) {
        if (caracterespermitidos.includes(cadena[i])) {
            result += cadena[i];
        }
    }
    return result;
    
}
export function ConvertStringToNumber(cadena) {
    const caracterespermitidos = "1234567890";
    var result = "";
    for (let i = 0; i < cadena.length; i++) {
        if (caracterespermitidos.includes(cadena[i])) {
            result += cadena[i];
        }
        else if (cadena[i] === ",") {
            result += ".";
        }
    }
    return result;
}