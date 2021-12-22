﻿
const MainProperties = {
    //訂單管理
    Order: {
        data: [
            {
                num: '',
                memberId: '',
                memberName: '',
                date: '',
                status: '',
                statusBadge: '',
                shippingMethod: '',
                shippingBadge: '',
                total: 0,
                isDel: 0,
                isPaid: 0
            }],

        postData: [
            {
                f_id: '',
                f_status: 0,
                f_shippingMethod: 0
            }],

        /*切換狀態*/
        STATUS: {
            tbd: 1,
            shipped: 2,
            transport: 3,
            returned: 4,
            chanceled: 5,
            abnormal: 6
        },

        SipMethod: {
            postm: 1,
            B2B: 2,
            privateCargo: 3
        }
        /*切換狀態*/
    }
}