
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
    },

    //帳號管理
    User: {
        data: [{
            id: 0,
            name: '',
            account: '',
            groupId: 0,
            groupName: 'Admin',
            createTime: '',
            updateTime: '',
        }],

        group: {
            '1': 'Admin',
            '2': 'Normal'
        },


        postData: {

        }
    },

    Member: {
        data: [{
            id: 0,
            name: '',
            account: '',
            level: 0,
            money: 0,
            isSuspend: 0,
        }],


        GetStatusCSS: function (statusCode) {

            const STATUS = {
                LV1: 1,
                LV2: 2,
                LV3: 3,
                LV4: 4,
                LV5: 5,
                LV6: 6
            }

            const statement = {
                [STATUS.LV1]: 'bg-info',
                [STATUS.LV2]: 'bg-primary',
                [STATUS.LV3]: 'bg-success',
                [STATUS.LV4]: 'bg-danger',
                [STATUS.LV5]: 'bg-secondary',
                [STATUS.LV6]: 'bg-warning'
            }

            return statement[statusCode]
            
        },




    }
}