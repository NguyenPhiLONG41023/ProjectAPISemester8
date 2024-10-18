window.onload = function () {
    new CustomerPage();
};

class CustomerPage {

    /**
     * Save data trong table
     * Author: NPLong (4/12/2023)
     */
    btnSaveOnClick() {
        try {
            //Thực hiện validate dữ liệu
            const validateRequired = this.validateData();
            if (validateRequired.errors.length > 0) {
                //focus vào ô lỗi đầu tiên
                const errorInputs = document.querySelectorAll(".input--invalid");
                if (errorInputs.length > 0) {
                    errorInputs[0].focus();
                }
            }
            else {
                // --> nếu dữ liệu đã hợp lệ thì gọi api thực hiện thêm mới
                //
            }

        } catch (error) {
            console.log(error)
        }
    }


    /**
     * Check những input bắt buộc nhập
     * Author: NPLong (5/12/2023)
     */
    checkRequiredInput() {
        try {
            let result = {
                inputValid: [],
                errors: [],
            };
            //Lấy ra tất cả input bắt buộc nhập
            let inputReqs = document.querySelectorAll("#dlgDetail input[required]");
            for (const input of inputReqs) {
                const value = input.value.trim();
                //input invalid
                if (value === "" || value === null || value === undefined) {
                    //chuyển border thành màu đỏ
                    input.classList.add("input--invalid");
                    //thêm thông báo phía dưới input
                    this.addErrorToInvalidInput(input);
                    result.inputValid.push(input);
                    result.errors.push("Không đc bỏ trống");
                } else { //input valid
                    //chuyển border về bình thường
                    input.classList.remove("input--invalid");
                    //xóa thông báo lỗi nếu tồn tại
                    if (input.nextElementSibling) {
                        input.nextElementSibling.remove();
                    }
                }
            }
            return result;
        } catch (error) {
            console.log(error);
        }
    }
}
