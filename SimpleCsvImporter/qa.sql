CREATE TABLE robot_t_qa (
	qaid CHAR(36) NOT NULL COMMENT '标识',
	question VARCHAR(256) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL COMMENT '问题',
	answer LONGTEXT CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL COMMENT '答案',
	keywords VARCHAR(64) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL COMMENT '关键词',
	bustype VARCHAR(32) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL COMMENT '业务类型',
	mode VARCHAR(16) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL COMMENT '机器人类型',
	lang VARCHAR(16) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT 'zh' NULL COMMENT '语言',
	robot VARCHAR(32) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL COMMENT '适配机器人',
	description VARCHAR(256) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL COMMENT '说明',
	promptcrispeid CHAR(36) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL COMMENT '提示词',
	recordtime datetime(6) DEFAULT CURRENT_TIMESTAMP(6)  NOT NULL COMMENT '记录时间',
	extraproperties json NULL COMMENT '扩展字段',
	concurrencystamp varchar(40) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL COMMENT '并发戳',
	rowversion timestamp(6) DEFAULT CURRENT_TIMESTAMP(6)  on update CURRENT_TIMESTAMP(6) NOT NULL COMMENT '数据版本',
	isdeleted bit(1) DEFAULT b'0' NOT NULL COMMENT '是否删除 0为否 1为是',
	creatorby varchar(64) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL COMMENT '创建人标识',
	creatorname varchar(64) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL COMMENT '创建人',
	createdon datetime(6) DEFAULT CURRENT_TIMESTAMP(6)  NOT NULL COMMENT '创建时间',
	deletedby varchar(64) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL COMMENT '删除人标识',
	deletedname varchar(64) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL COMMENT '删除人',
	deletedon datetime(6) NULL COMMENT '删除时间',
	lastmodifiedby varchar(64) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL COMMENT '修改人标识',
	lastmodifiedname varchar(64) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL COMMENT '修改人',
	lastmodifiedon datetime(6) NULL COMMENT '修改时间',
	CONSTRAINT robot_t_qa_pk PRIMARY KEY (qaid)
)
ENGINE=InnoDB
DEFAULT CHARSET=utf8mb4
COLLATE=utf8mb4_unicode_ci
COMMENT='问答记录信息表';


CREATE TABLE robot_t_prompt (
	promptcrispeid CHAR(36) NOT NULL COMMENT '提示词标识',
	context varchar(512) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL COMMENT '背景',
	roles varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL COMMENT '角色',
	steps varchar(512) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL COMMENT '步骤',
	persona varchar(256) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL COMMENT '人格',
	recordtime DATETIME(6) NULL COMMENT '记录时间',
	instruction json NOT NULL COMMENT '指令的投影',
	examples json NULL COMMENT '示例的投影',
	extraproperties json NULL COMMENT '扩展属性',
	rowversion timestamp(6) DEFAULT CURRENT_TIMESTAMP(6)  on update CURRENT_TIMESTAMP(6) NOT NULL COMMENT '数据版本',
	concurrencystamp varchar(40) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL COMMENT '时间戳',
	isdeleted bit(1) DEFAULT b'0' NOT NULL COMMENT '是否删除 0为否 1为是',
	creatorby varchar(64) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL COMMENT '创建人',
	creatorname varchar(64) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL COMMENT '创建人名称',
	createdon datetime(6) DEFAULT CURRENT_TIMESTAMP(6)  NOT NULL COMMENT '创建时间',
	deletedby varchar(64) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL COMMENT '删除人',
	deletedname varchar(64) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL COMMENT '删除人名称',
	deletedon datetime(6) NULL COMMENT '删除时间',
	lastmodifiedby varchar(64) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL COMMENT '修改人',
	lastmodifiedname varchar(64) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL COMMENT '修改人名称',
	lastmodifiedon datetime(6) NULL COMMENT '修改时间',
	tenantid varchar(64) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL COMMENT '租户标识',
	CONSTRAINT robot_t_prompt_pk PRIMARY KEY (promptcrispeid)
)
ENGINE=InnoDB
DEFAULT CHARSET=utf8mb4
COLLATE=utf8mb4_unicode_ci
COMMENT='提示词信息表';
